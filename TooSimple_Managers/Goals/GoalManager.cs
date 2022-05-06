using System.Net;
using TooSimple_DataAccessors.Database.FundingSchedules;
using TooSimple_DataAccessors.Database.Goals;
using TooSimple_Poco.Models.DataModels;
using TooSimple_Poco.Models.Dtos.Goals;
using TooSimple_Poco.Models.Entities;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Managers.Goals
{
    public class GoalManager : IGoalManager
    {
        private readonly IGoalAccessor _goalAccessor;
        private readonly IFundingScheduleAccessor _fundingScheduleAccessor;

        public GoalManager(IGoalAccessor goalAccessor, IFundingScheduleAccessor fundingScheduleAccessor)
        {
            _goalAccessor = goalAccessor;
            _fundingScheduleAccessor = fundingScheduleAccessor;
        }

        /// <summary>
        /// Retrieves goals from database.
        /// </summary>
        /// <param name="userId">
        /// User ID to retrieve goals for
        /// </param>
        /// <returns>
        /// GetGoalsDto with goals objects & http status code.
        /// </returns>
        public async Task<GetGoalsDto> GetGoalsByUserIdAsync(string userId)
        {
            IEnumerable<Goal> goals = await _goalAccessor.GetGoalsByUserIdAsync(userId);
            IEnumerable<FundingSchedule> fundingSchedules = await _fundingScheduleAccessor
                .GetFundingSchedulesByUserIdAsync(userId);

            if (!goals.Any() || !fundingSchedules.Any())
            {
                GetGoalsDto errorModel = new()
                {
                    ErrorMessage = "No goals found.",
                    Status = HttpStatusCode.NotFound,
                };

                return errorModel;
            }

            IEnumerable<GoalDataModel> dataModels = goals.Select(goal => new GoalDataModel(goal));

            foreach (GoalDataModel goalDataModel in dataModels)
            {
                FundingSchedule? fundingSchedule = fundingSchedules.FirstOrDefault(schedule =>
                    schedule.FundingScheduleId == goalDataModel.FundingScheduleId);

                if (fundingSchedule is null)
                {
                    GetGoalsDto errorModel = new()
                    {
                        ErrorMessage = "Something went wrong.",
                        Status = HttpStatusCode.InternalServerError,
                    };

                    return errorModel;
                }

                goalDataModel.FundingSchedule = new FundingScheduleDataModel(fundingSchedule);
            }

            GetGoalsDto getGoalDtos = new()
            {
                Success = true,
                Status = HttpStatusCode.OK,
                Goals = dataModels
            };

            return getGoalDtos;
        }

        /// <summary>
        /// Gets goal and its history from database.
        /// </summary>
        /// <param name="goalId">
        /// Goal ID to get data for.
        /// </param>
        /// <returns>
        /// DTO with http response message.
        /// </returns>
        public async Task<GetGoalDto> GetGoalByGoalIdAsync(string goalId)
        {
            Goal? goal = await _goalAccessor.GetGoalByGoalIdAsync(goalId);
            if (goal is null)
            {
                GetGoalDto errorModel = new()
                {
                    ErrorMessage = "No goal found.",
                    Status = HttpStatusCode.NotFound
                };

                return errorModel;
            }

            IEnumerable<FundingHistory> fundingHistory = await _goalAccessor
                .GetFundingHistoryByGoalId(goalId);

            if (fundingHistory.Any())
            {
                fundingHistory
                    .Where(fundingHistory => string.IsNullOrWhiteSpace(
                        fundingHistory.DestinationGoalName))
                    .ToList()
                    .ForEach(fundingHistory =>
                        fundingHistory.DestinationGoalName = "Safe to Spend");

                fundingHistory
                     .Where(fundingHistory => string.IsNullOrWhiteSpace(
                         fundingHistory.SourceGoalName))
                     .ToList()
                     .ForEach(fundingHistory =>
                         fundingHistory.SourceGoalName = "Safe to Spend");
            }

            IEnumerable<FundingSchedule>? fundingSchedules = await _fundingScheduleAccessor
                .GetFundingSchedulesByUserIdAsync(goal.UserAccountId);

            GoalDataModel goalDataModel = new(goal);

            IEnumerable<FundingScheduleDataModel> fundingScheduleDataModels = fundingSchedules
                .Select(schedule => new FundingScheduleDataModel(schedule));

            IEnumerable<FundingHistoryDataModel> fundingHistoryDataModels = fundingHistory
                .Select(history => new FundingHistoryDataModel(history));

            GetGoalDto responseModel = new()
            {
                Success = true,
                Status = HttpStatusCode.OK,
                Goal = goalDataModel,
                FundingHistory = fundingHistoryDataModels,
                FundingSchedules = fundingScheduleDataModels
            };

            return responseModel;
        }

        /// <summary>
        /// Calls Data Accessor to add new goal from user.
        /// Also checks to make sure required fields are present.
        /// </summary>
        /// <param name="goalDataModel">
        /// User submitted goal model.
        /// </param>
        /// <returns>
        /// <see cref="BaseHttpResponse"/> response with success or error messages.
        /// </returns>
        public async Task<BaseHttpResponse> AddNewGoalAsync(GoalDataModel goalDataModel)
        {
            List<string> errors = ValidateGoalAddOrUpdateRequest(goalDataModel);
            if (errors.Count > 0)
            {
                BaseHttpResponse errorResponse = new()
                {
                    ErrorMessage = string.Join(", ", errors),
                    Status = HttpStatusCode.BadRequest,
                    Success = false
                };

                return errorResponse;
            }

            goalDataModel.CreationDate = DateTime.UtcNow;
            goalDataModel.GetNextContribution();

            Goal goal = new(goalDataModel);

            DatabaseResponseModel responseModel = await _goalAccessor.AddNewGoalAsync(goal);
            
            if (!responseModel.Success)
            {
                BaseHttpResponse errorResponse = new()
                {
                    Success = false,
                    ErrorMessage = responseModel.ErrorMessage,
                    Status = HttpStatusCode.InternalServerError
                };

                return errorResponse;
            }

            BaseHttpResponse successResponse = new()
            {
                Success = true,
                Status = HttpStatusCode.Created
            };

            return successResponse;
        }

        /// <summary>
        /// Updates a goal with new data provided by user.
        /// </summary>
        /// <param name="goal">
        /// Goal with new data.
        /// </param>
        /// <returns>
        /// <see cref="BaseHttpResponse"/> with success or any error messages.
        /// </returns>
        public async Task<BaseHttpResponse> UpdateGoalAsync(GoalDataModel goalDataModel)
        {
            List<string> errors = ValidateGoalAddOrUpdateRequest(goalDataModel);
            if (string.IsNullOrWhiteSpace(goalDataModel.GoalId))
                errors.Add("Goal Id is required.");

            if (errors.Count > 0)
            {
                BaseHttpResponse errorResponse = new()
                {
                    ErrorMessage = string.Join(", ", errors),
                    Status = HttpStatusCode.BadRequest,
                    Success = false
                };

                return errorResponse;
            }
            
            Goal goal = new(goalDataModel);

            DatabaseResponseModel responseModel = await _goalAccessor.UpdateGoalAsync(goal);
            
            if (!responseModel.Success)
            {
                BaseHttpResponse errorResponse = new()
                {
                    Success = false,
                    ErrorMessage = responseModel.ErrorMessage,
                    Status = HttpStatusCode.InternalServerError
                };

                return errorResponse;
            }

            BaseHttpResponse successResponse = new()
            {
                Success = true,
                Status = HttpStatusCode.OK
            };

            return successResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goalId"></param>
        /// <returns></returns>
        public async Task<BaseHttpResponse> DeleteGoalAsync(string goalId)
        {
            Goal? goalDataModel = await _goalAccessor.GetGoalByGoalIdAsync(goalId);
            if (goalDataModel is null)
            {
                BaseHttpResponse errorResponse = new()
                {
                    Success = false,
                    ErrorMessage = "No goal was found with this id.",
                    Status = HttpStatusCode.NotFound
                };

                return errorResponse;
            }

            DatabaseResponseModel responseModel = await _goalAccessor.DeleteGoalAsync(goalId);

            if (!responseModel.Success)
            {
                BaseHttpResponse errorResponse = new()
                {
                    Success = false,
                    ErrorMessage = responseModel.ErrorMessage,
                    Status = HttpStatusCode.InternalServerError
                };

                return errorResponse;
            }

            BaseHttpResponse successResponse = new()
            {
                Success = true,
                Status = HttpStatusCode.OK
            };

            return successResponse;
        }

        /// <summary>
        /// Validates that required fields are present on user submitted goal model.
        /// </summary>
        /// <param name="goalDataModel">
        /// User submitted goal model.
        /// </param>
        /// <returns>
        /// List of error messages. If empty then sender was valid.
        /// </returns>
        private static List<string> ValidateGoalAddOrUpdateRequest(GoalDataModel goalDataModel)
        {
            List<string> errors = new();

            if (goalDataModel is null)
            {
                errors.Add("Goal model was not populated correctly");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(goalDataModel.GoalName))
                {
                    errors.Add("Goal name is required.");
                }

                if (string.IsNullOrWhiteSpace(goalDataModel.UserAccountId))
                {
                    errors.Add("User Account Id is required.");
                }

                if (string.IsNullOrWhiteSpace(goalDataModel.FundingScheduleId))
                {
                    errors.Add("Funding schedule is required.");
                }

                if (goalDataModel.DesiredCompletionDate <= DateTime.UtcNow)
                {
                    errors.Add("Completion date must be in the future.");
                }
            }

            return errors;
        }
    }
}
