using System.Net;
using TooSimple_DataAccessors.Database.Goals;
using TooSimple_Poco.Models.Budgeting;
using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Managers.Goals
{
    public class GoalManager : IGoalManager
    {
        private readonly IGoalAccessor _goalAccessor;

        public GoalManager(IGoalAccessor goalAccessor)
        {
            _goalAccessor = goalAccessor;
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
            IEnumerable<GoalDataModel> goals = await _goalAccessor.GetGoalsByUserIdAsync(userId);
            if (!goals.Any())
            {
                GetGoalsDto errorModel = new()
                {
                    ErrorMessage = "No goals found.",
                    Status = HttpStatusCode.NotFound,
                };

                return errorModel;
            }

            GetGoalsDto responseModel = new()
            {
                Success = true,
                Status = HttpStatusCode.OK,
                Goals = goals
            };

            return responseModel;
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
            GoalDataModel? goal = await _goalAccessor.GetGoalByGoalIdAsync(goalId);
            if (goal is null)
            {
                GetGoalDto errorModel = new()
                {
                    ErrorMessage = "No goal found.",
                    Status = HttpStatusCode.NotFound
                };

                return errorModel;
            }

            IEnumerable<FundingHistoryDataModel> fundingHistory = await _goalAccessor
                .GetFundingHistoryByGoalId(goalId);

            if (fundingHistory.Any())
            {
                fundingHistory
                    .Where(fundingHistory => string.IsNullOrWhiteSpace(
                        fundingHistory.DestinationGoalName))
                    .ToList()
                    .ForEach(fundingHistory =>
                        fundingHistory.DestinationGoalName = "Ready to Spend");

                fundingHistory
                     .Where(fundingHistory => string.IsNullOrWhiteSpace(
                         fundingHistory.SourceGoalName))
                     .ToList()
                     .ForEach(fundingHistory =>
                         fundingHistory.SourceGoalName = "Ready to Spend");
            }

            GetGoalDto responseModel = new()
            {
                Success = true,
                Status = HttpStatusCode.OK,
                Goal = goal,
                FundingHistory = fundingHistory
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
            
            DatabaseResponseModel responseModel = await _goalAccessor.AddNewGoalAsync(goalDataModel);
            
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

            DatabaseResponseModel responseModel = await _goalAccessor.UpdateGoalAsync(goalDataModel);
            
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
            GoalDataModel? goalDataModel = await _goalAccessor.GetGoalByGoalIdAsync(goalId);
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
