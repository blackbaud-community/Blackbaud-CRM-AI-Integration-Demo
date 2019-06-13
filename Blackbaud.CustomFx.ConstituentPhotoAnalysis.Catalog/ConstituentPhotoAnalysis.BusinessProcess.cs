using Blackbaud.AppFx.Server;
using Blackbaud.AppFx.Server.AppCatalog;
using Blackbaud.CustomFx.ConstituentPhotoAnalysis.SPWrapConstituentPhotoAnalysis;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.Data.SqlClient;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace Blackbaud.CustomFx.ConstituentPhotoAnalysis.Catalog
{
    public sealed class ConstituentPhotoAnalysisBusinessProcess : AppBusinessProcess
    {

        // You will need your own subscription key
        private const string subscriptionKey = "<Replace with your key>";

        // You must use the same region as you used to get your subscription
        // keys. For example, if you got your subscription keys from westus,
        // replace "westcentralus" with "westus".

        // Free trial subscription keys are generated in the westcentralus
        // region. If you use a free trial subscription key, you shouldn't
        // need to change the region.
        // Specify the Azure region
        private const string faceEndpoint = "https://westcentralus.api.cognitive.microsoft.com";

        private static readonly FaceAttributeType[] faceAttributes = {
            FaceAttributeType.Age,
            FaceAttributeType.Gender,
            FaceAttributeType.Emotion,
            FaceAttributeType.Smile
        };

        private static FaceClient faceClient;

        public override AppBusinessProcessResult StartBusinessProcess()
        {
            AppBusinessProcessResult result = new AppBusinessProcessResult()
            {
                OutputData = null,
                NumberSuccessfullyProcessed = 0,
                NumberOfExceptions = 0
            };

            faceClient = new FaceClient(
                new ApiKeyServiceClientCredentials(subscriptionKey),
                new System.Net.Http.DelegatingHandler[] { })
            {
                Endpoint = faceEndpoint
            };

            using (SqlConnection conn = RequestContext.OpenAppDBConnection())
            {
                List<Task> tasks = new List<Task>();
                Guid idSetRegisterID = Guid.Empty;
                SPWrapConstituentPhotoAnalysis.USR_USP_CONSTITUENTPHOTOANALYSISPROCESS_GETPARAMETERS.ResultRow parameterRow = SPWrapConstituentPhotoAnalysis.USR_USP_CONSTITUENTPHOTOANALYSISPROCESS_GETPARAMETERS.WrapperRoutines.ExecuteRow(conn, ProcessContext.ParameterSetID);
                if (parameterRow != null && (parameterRow.IDSETREGISTERID != Guid.Empty))
                {
                    idSetRegisterID = parameterRow.IDSETREGISTERID;
                }
                else
                {
                    throw new ServiceException("Could not retrieve parameters.");
                }

                SPWrapConstituentPhotoAnalysis.USR_USP_CONSTITUENT_GETPHOTOS.SPResultCollection rows = SPWrapConstituentPhotoAnalysis.USR_USP_CONSTITUENT_GETPHOTOS.WrapperRoutines.ExecuteSP(conn, idSetRegisterID);
                if (rows != null)
                {
                    foreach (SPWrapConstituentPhotoAnalysis.USR_USP_CONSTITUENT_GETPHOTOS.ResultRow row in rows.ResultSet)
                    {
                        tasks.Add(AnalyzePhoto(row, result));
                    }
                }
                
                // Use the business process timeout. If it times out before all tasks complete, errors will occur.
                Task.WhenAll(tasks).Wait(ProcessCommandTimeout * 1000);
            }


            return result;
        }

        // Detect faces in a local image
        private async Task AnalyzePhoto(SPWrapConstituentPhotoAnalysis.USR_USP_CONSTITUENT_GETPHOTOS.ResultRow row, AppBusinessProcessResult result)
        {
            if (row.PICTURE == null || row.PICTURE.Length == 0)
            {
                result.NumberOfExceptions++;
                return;
            }

            try
            {
                using (Stream imageStream = new MemoryStream(row.PICTURE))
                {
                    IList<DetectedFace> faceList =
                            await faceClient.Face.DetectWithStreamAsync(
                                imageStream, true, false, faceAttributes);

                    try
                    {
                        SaveConstituentPhotoAnalysis(faceList, row.ID);
                        result.NumberSuccessfullyProcessed++;
                    }
                    catch
                    {
                        result.NumberOfExceptions++;
                    }
                }
            }
            catch (APIErrorException e)
            {
                throw new Exception(e.Message);
            }
        }

        private void SaveConstituentPhotoAnalysis(IList<DetectedFace> faceList, Guid constituentID)
        {
            if (faceList == null || faceList.Count == 0)
            {
                throw new Exception("No faces found.");
            }
            else if (faceList.Count > 1)
            {
                throw new Exception("More than one face found.");
            }
            else
            {
                FaceAttributes faceAttributes = faceList[0].FaceAttributes;
                int age = -1;
                if (faceAttributes.Age.HasValue)
                {
                    age = (int)faceAttributes.Age.Value;
                }
                int genderCode = 0;
                if (faceAttributes.Gender.HasValue)
                {
                    switch (faceAttributes.Gender.ToString())
                    {
                        case "Unknown":
                            genderCode = 0;
                            break;
                        case "Male":
                            genderCode = 1;
                            break;
                        case "Female":
                            genderCode = 2;
                            break;
                    };
                }
                

                using (SqlConnection conn = RequestContext.OpenAppDBConnection())
                {
                    SPWrapConstituentPhotoAnalysis.USR_USP_CONSTITUENTPHOTOANALYSIS_UPSERT.WrapperRoutines.ExecuteNonQuery(
                        conn,
                        constituentID,
                        (byte) genderCode,
                        age,
                        (decimal)faceAttributes.Emotion.Anger,
                        (decimal)faceAttributes.Emotion.Contempt,
                        (decimal)faceAttributes.Emotion.Disgust,
                        (decimal)faceAttributes.Emotion.Fear,
                        (decimal)faceAttributes.Emotion.Happiness,
                        (decimal)faceAttributes.Emotion.Neutral,
                        (decimal)faceAttributes.Emotion.Sadness,
                        (decimal)faceAttributes.Emotion.Surprise,
                        null,
                        null
                    );
                }
            }

        }
    }
}