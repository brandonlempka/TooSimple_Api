CREATE TABLE FundingSchedules(
FundingScheduleId CHAR(40) PRIMARY KEY,
FundingScheduleName NVARCHAR(255),
Frequency int,
FirstContributionDate DATETIME,
UserAccountId CHAR(40),
foreign key (UserAccountId) references UserAccounts(UserAccountId)
);

CREATE TABLE Goals(
GoalId CHAR(40) PRIMARY KEY,
GoalName NVARCHAR(255),
GoalAmount DECIMAL(18,2),
DesiredCompletionDate DATETIME,
UserAccountId CHAR(40),
FundingScheduleId CHAR(40),
ExpenseFlag boolean,
RecurrenceTimeFrame int,
CreationDate datetime,
IsPaused boolean,
AutoSpendMerchantName nvarchar(255),
AmountContributed decimal(18,2),
AmountSpent decimal(18,2),
IsAutoRefillEnabled boolean,
NextContributionAmount decimal(18,2),
NextContributionDate datetime,
IsContributionFixed boolean,
foreign key (FundingScheduleId) references FundingSchedules(FundingScheduleId),
foreign key (UserAccountId) references UserAccounts(UserAccountId)
);
