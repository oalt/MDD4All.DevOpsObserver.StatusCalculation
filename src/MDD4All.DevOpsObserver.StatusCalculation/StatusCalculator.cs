using MDD4All.DevOpsObserver.DataModels;
using System;
using System.Collections.Generic;

namespace MDD4All.DevOpsObserver.StatusCalculation
{
    public class StatusCalculator
    {

        public DevOpsStatus CalculateOverallState(List<DevOpsStatusInformation> statusInformationList)
        {
            DevOpsStatus result = DevOpsStatus.Unknown;

            foreach(DevOpsStatusInformation statusInformation in statusInformationList)
            {
                DevOpsStatus elementStatus = CalculateDisplayStatus(statusInformation);

                switch(result)
                {
                    case DevOpsStatus.Unknown:
                        if(elementStatus != DevOpsStatus.Unknown)
                        {
                            result = elementStatus;
                        }
                        break;

                    case DevOpsStatus.Success:
                        if(elementStatus == DevOpsStatus.Warning || elementStatus == DevOpsStatus.Fail || elementStatus == DevOpsStatus.Error)
                        {
                            result = elementStatus;
                        }
                        break;

                    case DevOpsStatus.Fail:
                        if(elementStatus == DevOpsStatus.Error)
                        {
                            result = elementStatus;
                        }
                        break;
                }
                if(result == DevOpsStatus.Error)
                {
                    break; // loop optimization
                }
            }

            //if(result == DevOpsStatus.Unknown)
            //{
            //    result = DevOpsStatus.Success;
            //}

            return result;
        }

        public DevOpsStatus CalculateDisplayStatus(DevOpsStatusInformation statusInformation)
        {
            DevOpsStatus result = statusInformation.StatusValue;

            // if failed builds are younger than 7 days, return status = Warning
            if(statusInformation.StatusValue == DevOpsStatus.Error || statusInformation.StatusValue == DevOpsStatus.Fail)
            {
                if(!IsFailedSinceSevenDays(statusInformation))
                {
                    result = DevOpsStatus.Warning;
                }
            }

            return result;
        }


        private bool IsFailedSinceSevenDays(DevOpsStatusInformation statusInformation)
        {
            bool result = false;

            TimeSpan timeDifference = DateTime.Now.Subtract(statusInformation.BuildTime);

            // TODO Weekend calculation
            if (timeDifference.Days >= 7)
            {
                result = true;
            }

            return result;
        }
    }
}
