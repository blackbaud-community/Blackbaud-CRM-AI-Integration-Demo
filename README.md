# Blackbaud-CRM-AI-Integration-Demo
Microsoft offers a variety of [Cognitive Services](https://docs.microsoft.com/en-us/azure/cognitive-services/). These services allow you to utilize AI technologies like computer vision and text analytics without needing a background in artificial intelligence or data science.
Using Blackbaud CRM's SDK, you can integrate with these services to gain more intelligence from your data. This code sample will demonstrate one such integration, using Microsoft's Face API to analyze constituent photos.
## Prerequisites
*	An installation of CRM
*	Administrator-level access rights to said installation
*	Access to said installation's database and virtual directory
*	A specified and valid probe path in your installation's web config file
*	A Face API subscription key (learn more on Microsoft's [Face API Quickstart Guide](https://docs.microsoft.com/en-us/azure/cognitive-services/face/QuickStarts/CSharp))
* Visual Studio 2015 or 2017
* Familiarity with creating customizations, including pages, forms, data lists, CLR operations, and business processes.
## Limitations and Clarifications
* The Constituent Page changes provided in this sample are included as a spec. Using LoadSpec the page spec will overwrite any other Constituent page customizations you have. So you may wish to alter the page manually.
* This sample uses SPWrap to wrap procedures for use in the Catalog. SPWrap doesn't handle procedures prefixed with "USR_" without some manual effort. If you wish to extend this sample, use SQL commands instead of SPWrap.
* This sample sends each image off to be analyzed using parallel tasks. There is a long wait block to allow each task to finish. If they do not finish before the timeout, the connection will be closed. Any attempts by the tasks to use the connection after this point will result in an error.
* This sample will only handle photos with one face. A photo with multiple faces or no face will result in a row-level error.
* This sample is for reference purposes only! It has not been thoroughly tested or reviewed and is offered without a guarantee of any kind. If you choose to implement it, be sure to update it to meet your organization's security/compliance rules.
## Writing the Code
This sample assumes you are familiar with customizations in CRM. If you have questions about how to use the CRM SDK to write new features, please visit the [Infinity Development Guide](https://www.blackbaud.com/files/support/guides/infinitydevguide/infsdk-developer-help.htm).
### The Constituent Photo Analysis Table
Before we send constituent photos off to the Face API, we will need a place to store the results we get back. Let's create the Constituent Photo Analysis Table.
``` xml
<TableSpec
  xmlns="bb_appfx_table"
  xmlns:c="bb_appfx_commontypes"
  ID="9ce8d45a-c025-4832-aebf-613d69574144"
  Name="Constituent Photo Analysis"
  Description="Stores information about analyzed constituent photos."
  Author="Blackbaud Demo"
  Tablename="USR_CONSTITUENTPHOTOANALYSIS"
  IsBuiltIn="false"
  PrimaryKeyAsForeignKeyTablename="CONSTITUENT"
  >

  <Fields>
    <EnumField Name="PREDICTEDGENDERCODE" Description="The predicted gender of the constituent." DefaultValue="0" >
      <EnumValues>
        <EnumValue ID="0" Translation="Unknown"/>
        <EnumValue ID="1" Translation="Male"/>
        <EnumValue ID="2" Translation="Female"/>
        <EnumValue ID="3" Translation="Other"/>
      </EnumValues>
    </EnumField>

    <NumberField Name="ESTIMATEDAGE" Description="The estimated age of the constituent." />

    <DecimalField Name="ANGERRATING" Description="The estimated confidence (0-1) that the person in the photo has an expression of anger." />

    <DecimalField Name="CONTEMPTRATING" Description="The estimated confidence (0-1) that the person in the photo has an expression of contempt." />

    <DecimalField Name="DISGUSTRATING" Description="The estimated confidence (0-1) that the person in the photo has an expression of disgust." />

    <DecimalField Name="FEARRATING" Description="The estimated confidence (0-1) that the person in the photo has an expression of fear." />

    <DecimalField Name="HAPPINESSRATING" Description="The estimated confidence (0-1) that the person in the photo has an expression of happiness." />

    <DecimalField Name="NEUTRALRATING" Description="The estimated confidence (0-1) that the person in the photo has a neutral expression." />

    <DecimalField Name="SADNESSRATING" Description="The estimated confidence (0-1) that the person in the photo has an expression of sadness." />

    <DecimalField Name="SURPRISERATING" Description="The estimated confidence (0-1) that the person in the photo has an expression of surprise." />
  </Fields>

</TableSpec>
```
This table will store the predicted gender, age, and emotion features of a photo.
### The Constituent Photo Analysis Update Stored Procedure
After creating a place to store the data, we will need to write a procedure to update it.
``` xml
<SQLStoredProcedureSpec
xmlns="bb_appfx_sqlstoredprocedure"
xmlns:c="bb_appfx_commontypes"
ID="e4b6807c-3918-4767-a6ef-fa37b62824c8"
Name="USR_USP_CONSTITUENTPHOTOANALYSIS_UPSERT"
Description="Add or update the analysis of a constituent photo."
Author="Blackbaud Demo"
SPName="USR_USP_CONSTITUENTPHOTOANALYSIS_UPSERT"
>

  <CreateProcedureSQL>
    <![CDATA[
create procedure dbo.USR_USP_CONSTITUENTPHOTOANALYSIS_UPSERT(
  @CONSTITUENTID uniqueidentifier,
  @PREDICTEDGENDERCODE tinyint,
  @ESTIMATEDAGE int,
  @ANGERRATING decimal(20, 4),
  @CONTEMPTRATING decimal(20, 4),
  @DISGUSTRATING decimal(20, 4),
  @FEARRATING decimal(20, 4),
  @HAPPINESSRATING decimal(20, 4),
  @NEUTRALRATING decimal(20, 4),
  @SADNESSRATING decimal(20, 4),
  @SURPRISERATING decimal(20, 4),
  @CHANGEAGENTID uniqueidentifier,
  @CURRENTDATE datetime
)
as
begin
  set nocount on;
	
  if @CHANGEAGENTID is null
    exec USP_CHANGEAGENT_GETORCREATECHANGEAGENT @CHANGEAGENTID output;

  if @CURRENTDATE is null 
    set @CURRENTDATE = getdate();

  merge into dbo.USR_CONSTITUENTPHOTOANALYSIS as TARGET
  using (
    select @CONSTITUENTID CONSTITUENTID
  ) as SOURCE
  on (
    TARGET.ID = SOURCE.CONSTITUENTID 
  )
  when matched then 
	  update set 
      TARGET.PREDICTEDGENDERCODE = @PREDICTEDGENDERCODE,
      TARGET.ESTIMATEDAGE = @ESTIMATEDAGE,
      TARGET.ANGERRATING = @ANGERRATING,
      TARGET.CONTEMPTRATING = @CONTEMPTRATING,
      TARGET.DISGUSTRATING = @DISGUSTRATING,
      TARGET.FEARRATING = @FEARRATING,
      TARGET.HAPPINESSRATING = @HAPPINESSRATING,
      TARGET.NEUTRALRATING = @NEUTRALRATING,
      TARGET.SADNESSRATING = @SADNESSRATING,
      TARGET.SURPRISERATING = @SURPRISERATING,
      TARGET.CHANGEDBYID = @CHANGEAGENTID,
      TARGET.DATECHANGED = @CURRENTDATE
  when not matched then
	  insert
    (
      ID,
      PREDICTEDGENDERCODE,
      ESTIMATEDAGE,
      ANGERRATING,
      CONTEMPTRATING,
      DISGUSTRATING,
      FEARRATING,
      HAPPINESSRATING,
      NEUTRALRATING,
      SADNESSRATING,
      SURPRISERATING,
      ADDEDBYID,
      CHANGEDBYID,
      DATEADDED,
      DATECHANGED
    )
    values
    (
      @CONSTITUENTID,
      @PREDICTEDGENDERCODE,
      @ESTIMATEDAGE,
      @ANGERRATING,
      @CONTEMPTRATING,
      @DISGUSTRATING,
      @FEARRATING,
      @HAPPINESSRATING,
      @NEUTRALRATING,
      @SADNESSRATING,
      @SURPRISERATING,
      @CHANGEAGENTID,
      @CHANGEAGENTID,
      @CURRENTDATE,
      @CURRENTDATE
    );
end
		]]>
  </CreateProcedureSQL>

</SQLStoredProcedureSpec>
```
This procedure uses a simple merge operation to either add or update the results of the constituent photo analysis.
### The Constituent Photo Analysis View Form
We will also need to be able to retrieve the data from the table so that we can view it.
``` xml
<ViewDataFormTemplateSpec
  xmlns="bb_appfx_viewdataformtemplate"
  xmlns:c="bb_appfx_commontypes"
  ID="13e631e3-a570-4f95-a65d-653a1020418e"
  Name="Constituent Photo Analysis View Form"
  Description="A data form for viewing the results of a constituent photo analysis."
  Author="Blackbaud Demo"
  DataFormInstanceID="c8eb35c2-6fda-4f1e-a379-b7609aca772f"
  RecordType="Constituent"
  c:SecurityUIFolder="Constituent Photo Analysis"
  >

  <SPDataForm SPName="USR_USP_DATAFORMTEMPLATE_VIEW_CONSTITUENTPHOTOANALYSIS">
    <c:CreateProcedureSQL>
      <![CDATA[
create procedure dbo.USR_USP_DATAFORMTEMPLATE_VIEW_CONSTITUENTPHOTOANALYSIS
(
  @ID uniqueidentifier,
  @DATALOADED bit = 0 output,
  @PREDICTEDGENDER nvarchar(7) = null output,
  @ESTIMATEDAGE int = null output,
  @ANGERRATING  decimal(20, 4) = null output,
  @CONTEMPTRATING  decimal(20, 4) = null output,
  @DISGUSTRATING  decimal(20, 4) = null output,
  @FEARRATING  decimal(20, 4) = null output,
  @HAPPINESSRATING  decimal(20, 4) = null output,
  @NEUTRALRATING  decimal(20, 4) = null output,
  @SADNESSRATING  decimal(20, 4) = null output,
  @SURPRISERATING  decimal(20, 4) = null output
)
as
  set nocount on;

  set @DATALOADED = 0;
	
  select 
    @DATALOADED = 1,
    @PREDICTEDGENDER = PREDICTEDGENDER,
    @ESTIMATEDAGE = ESTIMATEDAGE,
    @ANGERRATING = ANGERRATING,
    @CONTEMPTRATING = CONTEMPTRATING,
    @DISGUSTRATING = DISGUSTRATING,
    @FEARRATING = FEARRATING,
    @HAPPINESSRATING = HAPPINESSRATING,
    @NEUTRALRATING = NEUTRALRATING,
    @SADNESSRATING = SADNESSRATING,
    @SURPRISERATING = SURPRISERATING
  from 
    dbo.CONSTITUENT
    left join dbo.USR_CONSTITUENTPHOTOANALYSIS on USR_CONSTITUENTPHOTOANALYSIS.ID = CONSTITUENT.ID
  where 
    CONSTITUENT.ID = @ID;
	
  return 0;
]]>
    </c:CreateProcedureSQL>
  </SPDataForm>

  <FormMetaData xmlns="bb_appfx_commontypes">
    <FormFields>
      <FormField FieldID="PREDICTEDGENDER" Caption="Gender" DataType="String" MaxLength="7" />
      <FormField FieldID="ESTIMATEDAGE" Caption="Estimated age" DataType="Integer" />
      <FormField FieldID="ANGERRATING" Caption="Anger" DataType="Decimal" Precision="20" Scale="4" />
      <FormField FieldID="CONTEMPTRATING" Caption="Contempt" DataType="Decimal" Precision="20" Scale="4" />
      <FormField FieldID="DISGUSTRATING" Caption="Disgust" DataType="Decimal" Precision="20" Scale="4" />
      <FormField FieldID="FEARRATING" Caption="Fear" DataType="Decimal" Precision="20" Scale="4" />
      <FormField FieldID="HAPPINESSRATING" Caption="Happiness" DataType="Decimal" Precision="20" Scale="4" />
      <FormField FieldID="NEUTRALRATING" Caption="Neutral" DataType="Decimal" Precision="20" Scale="4" />
      <FormField FieldID="SADNESSRATING" Caption="Sadness" DataType="Decimal" Precision="20" Scale="4" />
      <FormField FieldID="SURPRISERATING" Caption="Surprise" DataType="Decimal" Precision="20" Scale="4" />
    </FormFields>
  </FormMetaData>

</ViewDataFormTemplateSpec>
```
### Updating the Constituent Page
Once we have our table, stored procedure, and view form, we will need to add the view form to the Constituent Page or another page of your choice. To do so, perform the following steps.
* Go to the page
* Enter Design Mode
* Click "Edit tabs" at the top of the page
* Click "Add"
* Choose a caption like "Constituent Photo Analysis"
* Scroll down to "Sections" and click the "..." option.
* Click "Add"
* Scroll down to "DataForm" and search for "Constituent Photo Analysis View Form"
* Save everything
The results will be blank for now. Don't worry. They will populate once the process is run.
### The Constituent Photo Analysis Business Process
To analyze constituent photos, you will need to create a business process to send them to Microsoft's Face API. We will skip the related business process specs and focus on the business process code itself.
First, you will need to fill in the subscription key and subscription region you got when you signed up.
``` cs
// You will need your own subscription key
private const string subscriptionKey = "<your key here>";

//You must use the same region as you used to get your subscription
// keys. For example, if you got your subscription keys from westus,
// replace "westcentralus" with "westus".

// Free trial subscription keys are generated in the westcentralus
// region. If you use a free trial subscription key, you shouldn't
// need to change the region.
// Specify the Azure region
private const string faceEndpoint = "https://westcentralus.api.cognitive.microsoft.com";
```
Next, you will need the code that pulls constituent photos from CRM and instantiates tasks to process them.
``` cs
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
        throw new System.Exception("Could not retrieve parameters.");
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
```
The first procedure pulls the constituent selection from the business process instance. The second procedure gets the photos and passes them off to the AnalyzePhoto function for processing. The final step waits for all the photos to be processed or for the business process to timeout. This timeout is quite long, and timeouts are unlikely.
### AnalyzePhoto
The AnalyzePhoto function takes in a photo - a byte array - and sends it off to the Face API for processing. If it successfully gets a response, it will save the analysis back to the CRM database.
``` cs
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
```
### SaveConstituentPhotoAnalysis
The final procedure processes the results returned from the Face API and uses the stored procedure you created earlier to save the data to CRM. 
``` cs
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
```
# Testing Your Code
Place all of the code in a location of your choice. Update both projects' build output paths to be your bin\custom folder. Ensure that your installation's web.config is properly set up to look in the bin\custom folder. If you have not configured your web.config, follow Steps 7 and 8 on [this page](https://www.blackbaud.com/files/support/guides/infinitydevguide/infsdk-developer-help.htm#../Subsystems/infps-developer-help/Content/Exercises/coexBuildingAndDeployingTheFoodBankPackageSpec1.htm?Highlight=%22bin\custom%22).
Once everything is built, load all of the specs using the Constituent Photo Analysis package spec. Make sure you do not have the "/nodepends" flag on your LoadSpec command or it will not load the other specs. If LoadSpec complains about missing the DLL, you will need to make a copy of that DLL in the location it's looking.
After everything is loaded and you've customized your constituent page with the Constituent Photo Analysis view form, add photos to some constituent records and create a selection for those records. Locate the "Constituent Photo Analysis" task in the Constituent functional area. Create a business process, choose your selection, and run it. Once it completes, check the Constituent Photo Analysis tab on the Constituent page and view your results!
