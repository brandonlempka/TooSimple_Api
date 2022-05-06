using TooSimple_Poco.Models.Entities;

namespace TooSimple_Poco.Models.DataModels
{
    public class FundingScheduleDataModel
	{
        public string FundingScheduleId { get; set; } = string.Empty;
        public string UserAccountId { get; set; } = string.Empty;
        public string FundingScheduleName { get; set; } = string.Empty;
        public int Frequency { get; set; }
        public DateTime FirstContributionDate { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FundingScheduleDataModel() { }

        /// <summary>
        /// Constructor for converting <see cref="FundingSchedule"/>
        /// from database to <see cref="FundingScheduleDataModel"/>
        /// </summary>
        /// <param name="fundingSchedule">
        /// <see cref="FundingSchedule"/> from database.
        /// </param>
        public FundingScheduleDataModel(FundingSchedule fundingSchedule)
        {
            FundingScheduleId = fundingSchedule.FundingScheduleId;
            UserAccountId = fundingSchedule.UserAccountId;
            FundingScheduleName = fundingSchedule.FundingScheduleName;
            Frequency = fundingSchedule.Frequency;
            FirstContributionDate = fundingSchedule.FirstContributionDate;
        }
    }
}
