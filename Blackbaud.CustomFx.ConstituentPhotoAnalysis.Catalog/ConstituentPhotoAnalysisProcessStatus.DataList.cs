using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Blackbaud.AppFx.Server;
using Blackbaud.AppFx.Server.AppCatalog;
using Blackbaud.AppFx.SpWrap;



namespace Blackbaud.CustomFx.ConstituentPhotoAnalysis.Catalog
{

    public sealed class ConstituentPhotoAnalysisProcessStatusDataList : AppDataList
    {
        public Nullable<int> StatusCode;

        private enum ProcessStatusImage
        {
            check = 0,
            businessprocessspec,
            warning,
            x_16
        }

        private enum FieldList
        {
            ID,
            BUSINESSPROCESSCATALOGNAME,
            STATUSCODE,
            STATUS,
            ERRORMESSAGE,
            USERNAME,
            STARTEDON,
            ENDEDON,
            NUMBERPROCESSED,
            NUMBEROFEXCEPTIONS,
            SERVERNAME,
            DURATION,
            HASLETTERTEMPLATE,
            HASLABELTEMPLATE,
            BUSINESSPROCESSCATALOGID,
            PARAMETERSETID,
            OUTPUTTABLEEXISTS
        }

        public override AppDataListResult GetListResults()
        {
            List<DataListResultRow> resultList = new List<DataListResultRow>();
            int timeOut = this.RequestContext.ClientAppInfo.TimeOutSeconds;

            if (! StatusCode.HasValue)
            {
                StatusCode = -1;
            }

            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.AppendLine("select");
            builder.AppendLine("  BUSINESSPROCESSSTATUS.ID,");
            builder.AppendLine("  BUSINESSPROCESSCATALOG.NAME as BUSINESSPROCESSCATALOGNAME,");
            builder.AppendLine("  BUSINESSPROCESSSTATUS.STATUSCODE,");
            builder.AppendLine("  BUSINESSPROCESSSTATUS.STATUS,");
            builder.AppendLine("  BUSINESSPROCESSSTATUS.ERRORMESSAGE,");
            builder.AppendLine("  dbo.UFN_APPUSER_CUSTOMAUTHUSERID_OR_USERNAME(STARTEDBY.ID) AS USERNAME,");
            builder.AppendLine("  BUSINESSPROCESSSTATUS.STARTEDON,");
            builder.AppendLine("  BUSINESSPROCESSSTATUS.ENDEDON,");
            builder.AppendLine("  BUSINESSPROCESSSTATUS.NUMBERPROCESSED,");
            builder.AppendLine("  BUSINESSPROCESSSTATUS.NUMBEROFEXCEPTIONS,");
            builder.AppendLine("  BUSINESSPROCESSSTATUS.SERVERNAME,");
            builder.AppendLine("  datediff(s, BUSINESSPROCESSSTATUS.STARTEDON, coalesce(BUSINESSPROCESSSTATUS.ENDEDON, getdate())) DURATION,");
            builder.AppendLine("  case when BUSINESSPROCESSSTATUS.LETTERTEMPLATE is null then 0 else 1 end as HASLETTERTEMPLATE,");
            builder.AppendLine("  case when BUSINESSPROCESSSTATUS.LABELTEMPLATE is null then 0 else 1 end as HASLABELTEMPLATE,");
            builder.AppendLine("  BUSINESSPROCESSCATALOG.ID as BUSINESSPROCESSCATALOGID,");
            builder.AppendLine("  STATUS.PARAMETERSETID,");
            builder.AppendLine("  cast((case when exists(select * from dbo.BUSINESSPROCESSOUTPUT where BUSINESSPROCESSOUTPUT.BUSINESSPROCESSSTATUSID = BUSINESSPROCESSSTATUS.ID) then 1 else 0 end) as bit) as OUTPUTTABLEEXISTS");
            builder.AppendLine("from dbo.BUSINESSPROCESSSTATUS");
            builder.AppendLine("inner join dbo.USR_CONSTITUENTPHOTOANALYSISPROCESSSTATUS as STATUS on STATUS.ID = BUSINESSPROCESSSTATUS.ID");
            builder.AppendLine("inner join dbo.APPUSER as STARTEDBY on STARTEDBY.ID = BUSINESSPROCESSSTATUS.STARTEDBYUSERID");
            builder.AppendLine("inner join dbo.BUSINESSPROCESSCATALOG on BUSINESSPROCESSCATALOG.ID = BUSINESSPROCESSSTATUS.BUSINESSPROCESSCATALOGID");
            builder.AppendLine("left join dbo.[BUSINESSPROCESSSTATUS_EXT] on [BUSINESSPROCESSSTATUS_EXT].[ID] = [BUSINESSPROCESSSTATUS].[ID]");
            builder.AppendLine("where STATUS.PARAMETERSETID = @PARAMETERSETID");
            builder.AppendLine("and dbo.UFN_SECURITY_APPUSER_GRANTED_BUSINESSPROCESSINSTANCE_IN_SYSTEMROLE(@APPUSERID, STATUS.PARAMETERSETID) = 1");
            if (StatusCode > -1) 
            {
                builder.AppendLine("and BUSINESSPROCESSSTATUS.STATUSCODE = @STATUSCODE");

            }
            builder.AppendLine("order by BUSINESSPROCESSSTATUS.STARTEDON desc");

            using (SqlConnection conn = this.RequestContext.OpenAppDBConnection())
            {
                int returnValue = 0;
                Blackbaud.AppFx.SpWrap.USP_BUSINESSPROCESSSTATUS_VALIDATESTATUS.WrapperRoutines.ExecuteNonQuery(conn, ref returnValue, timeOut);

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = builder.ToString();
                    cmd.CommandTimeout = timeOut;
                    cmd.Parameters.AddWithValue("@PARAMETERSETID", this.ProcessContext.ContextRecordID);
                    if (StatusCode > -1)
                    {
                        cmd.Parameters.AddWithValue("@STATUSCODE", StatusCode);
                    }
                    cmd.Parameters.AddWithValue("@APPUSERID", this.RequestContext.AppUserInfo.AppUserDBId);

                    using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            DataListResultRow result = new DataListResultRow();
                            string statusMessage = string.Empty;
                            string duration = string.Empty;

                            int.TryParse(reader.GetValue((int) FieldList.STATUSCODE).ToString(), out int statusCode);

                            switch(statusCode)
                            {
                                case 0: 
                                case 3:
                                case 4: // Completed or Results Cleared
                                    statusMessage = reader.GetString((int) FieldList.STATUS);
                                    break;
                                case 1: // Running
                                    using (SqlConnection statusConn = this.RequestContext.OpenAppDBConnection())
                                    {
                                        using (SqlCommand statusCmd = statusConn.CreateCommand())
                                        {
                                            statusCmd.CommandText = String.Format("select STATUSMESSAGE from ##BUSINESSPROCESSSTATUS_{0}", reader.GetGuid((int) FieldList.ID).ToString().Replace("-", "_"));
                                            statusCmd.CommandTimeout = timeOut;
                                            statusMessage = statusCmd.ExecuteScalar().ToString();
                                        }                                        
                                    }
                                    break;
                                case 2: // Did not finish
                                    statusMessage = reader.GetString((int) FieldList.ERRORMESSAGE);
                                    break;
                            }

                            TimeSpan durationSpan = new TimeSpan(0, 0, (int) reader.GetSqlInt32((int) FieldList.DURATION));

                            System.Text.StringBuilder durationBuilder = new System.Text.StringBuilder();
                            if (durationSpan.Days > 1)
                            {
                                durationBuilder.AppendFormat("{0} {1} ", durationSpan.Days, "days");
                            } 
                            else if (durationSpan.Days > 0)
                            {
                                durationBuilder.AppendFormat("{0} {1} ", durationSpan.Days, "day");
                            }


                            if (durationSpan.Hours > 1)
                            {
                                durationBuilder.AppendFormat("{0} {1} ", durationSpan.Hours, "hours");
                            } 
                            else if (durationSpan.Hours > 0)
                            {
                                durationBuilder.AppendFormat("{0} {1} ", durationSpan.Hours, "hour");
                            }

                            if (durationSpan.Minutes > 1)
                            {
                                durationBuilder.AppendFormat("{0} {1} ", durationSpan.Minutes, "minutes");
                            }
                            else if (durationSpan.Minutes > 0)
                            {
                                durationBuilder.AppendFormat("{0} {1} ", durationSpan.Minutes, "minute");
                            }

                            if (durationSpan.Seconds > 1)
                            {
                                durationBuilder.AppendFormat("{0} {1} ", durationSpan.Seconds, "seconds");
                            }
                            else if (durationSpan.Seconds > 0)
                            {
                                durationBuilder.AppendFormat("{0} {1} ", durationSpan.Seconds, "second");
                            }

                            duration = durationBuilder.ToString().Trim();

                            if (string.IsNullOrEmpty(duration))
                            {
                                duration = "0 seconds";
                            }

                            int numberProcessed;
                            int numberOfExceptions;

                            numberProcessed = reader.GetInt32((int) FieldList.NUMBERPROCESSED);
                            numberOfExceptions = reader.GetInt32((int) FieldList.NUMBEROFEXCEPTIONS);

                            string imageKey;

                            if (numberOfExceptions == 0)
                            {
                                if (statusCode == 4) //enqueued uses the businessprocessspec image key
                                {
                                    imageKey = "RES:businessprocessspec";
                                } 
                                else 
                                {
                                    imageKey = String.Format("RES:{0}", (ProcessStatusImage) statusCode).ToString();
                                }
                            }
                            else
                            {
                                imageKey = String.Format("RES:{0}", ProcessStatusImage.warning.ToString());
                            }

                            List<string> valueList = new List<string>
                            {
                                reader.GetGuid((int)FieldList.ID).ToString(),
                                reader.GetString((int)FieldList.BUSINESSPROCESSCATALOGNAME),
                                reader.GetString((int)FieldList.STATUS),
                                statusMessage,
                                reader.GetString((int)FieldList.USERNAME),
                                FormatFieldForList(reader.GetDateTime((int)FieldList.STARTEDON))
                            };
                            if (statusCode != 1 && statusCode != 4)
                            {
                                valueList.Add(FormatFieldForList(reader.GetDateTime((int) FieldList.ENDEDON)));
                            }
                            else
                            {
                                valueList.Add(String.Empty);
                            } 
                            valueList.Add(duration);
                            valueList.Add(numberProcessed.ToString());
                            valueList.Add(numberOfExceptions.ToString());
                            valueList.Add((numberProcessed + numberOfExceptions).ToString());
                            valueList.Add(reader.GetString((int) FieldList.SERVERNAME));
                            valueList.Add(imageKey);
                            valueList.Add((statusCode = 0).ToString());
                            valueList.Add(((statusCode == 0) && (numberProcessed > 0)).ToString());
                            valueList.Add(((statusCode == 0) && (numberProcessed > 0) && (reader.GetInt32((int)FieldList.HASLETTERTEMPLATE) == 1)).ToString());
                            valueList.Add(((statusCode == 0) && (numberProcessed > 0) && (reader.GetInt32((int)FieldList.HASLABELTEMPLATE) == 1)).ToString());
                            valueList.Add("False");
                            valueList.Add("False");
                            valueList.Add(reader.GetGuid((int) FieldList.BUSINESSPROCESSCATALOGID).ToString());
                            valueList.Add(reader.GetGuid((int) FieldList.PARAMETERSETID).ToString());
                            valueList.Add(reader.GetBoolean((int) FieldList.OUTPUTTABLEEXISTS).ToString());

                            result.Values = valueList.ToArray();

                            resultList.Add(result);
                        }
                    }
                }                
            }
            return new AppDataListResult(resultList);
        }
    }
}