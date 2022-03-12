CREATE TABLE PlaidAccountTypes(
PlaidAccountTypeId INT PRIMARY KEY auto_increment,
PlaidAccountTypeName NVARCHAR(255));

insert into PlaidAccountTypes (PlaidAccountTypeName) VALUES (
'UNKNOWN');
insert into PlaidAccountTypes (PlaidAccountTypeName) VALUES (
'Checking');
insert into PlaidAccountTypes (PlaidAccountTypeName) VALUES (
'Savings');

CREATE TABLE PlaidAccounts(
	PlaidAccountId CHAR(40) NOT NULL PRIMARY KEY,
    PlaidAccountTypeId INT NOT NULL,
    UserAccountId CHAR(40) NOT NULL,
    Mask nvarchar(255),
    Name nvarchar(255),
    NickName nvarchar(255),
    CurrentBalance decimal(18,2) default 0.00,
    AvailableBalance decimal(18,2) default 0.00,
    CurrencyCode nvarchar(255),
    AccessToken nvarchar(255),
    LastUpdated datetime,
    IsActiveForBudgetingFeatures boolean,
    IsPlaidRelogRequired boolean,
    foreign key (PlaidAccountTypeId) references PlaidAccountTypes(PlaidAccountTypeId),
    foreign key (UserAccountId) references UserAccounts(UserAccountId)
);

