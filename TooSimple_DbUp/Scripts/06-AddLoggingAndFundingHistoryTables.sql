create table Logging (
LogId Int not null primary key auto_increment,
Code nvarchar(255),
Message nvarchar(20000),
MessageTime datetime
);

create table FundingHistory(
FundingHistoryId char(40) not null primary key,
SourceGoalId char(40),
DestinationGoalId char(40),
Amount decimal(18,2),
TransferDate datetime,
Note nvarchar(2000),
IsAutomatedTransfer boolean
);