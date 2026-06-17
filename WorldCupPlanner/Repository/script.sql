-- DROP SCHEMA dbo;

-- WorldCupPlannerDb.dbo.Fixtures definition

-- Drop table

-- DROP TABLE WorldCupPlannerDb.dbo.Fixtures;

CREATE TABLE WorldCupPlannerDb.dbo.Fixtures (
                                                Id int IDENTITY(1,1) NOT NULL,
                                                CurrentPhase int NOT NULL,
                                                SimulationEngineName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
                                                CONSTRAINT PK_Fixtures PRIMARY KEY (Id)
);


-- WorldCupPlannerDb.dbo.LogEntries definition

-- Drop table

-- DROP TABLE WorldCupPlannerDb.dbo.LogEntries;

CREATE TABLE WorldCupPlannerDb.dbo.LogEntries (
                                                  Id int IDENTITY(1,1) NOT NULL,
    [Timestamp] datetime2 NOT NULL,
                                                  UserEmail nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                                  Category nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [Action] nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    Detail nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    CONSTRAINT PK_LogEntries PRIMARY KEY (Id)
    );


-- WorldCupPlannerDb.dbo.Notifications definition

-- Drop table

-- DROP TABLE WorldCupPlannerDb.dbo.Notifications;

CREATE TABLE WorldCupPlannerDb.dbo.Notifications (
                                                     Id uniqueidentifier NOT NULL,
                                                     IsRead bit NOT NULL,
                                                     Message nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                                     ReadAt datetime2 NULL,
                                                     RecipientEmail nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [Timestamp] datetime2 NOT NULL,
                                                     CONSTRAINT PK_Notifications PRIMARY KEY (Id)
);


-- WorldCupPlannerDb.dbo.Stadiums definition

-- Drop table

-- DROP TABLE WorldCupPlannerDb.dbo.Stadiums;

CREATE TABLE WorldCupPlannerDb.dbo.Stadiums (
                                                StadiumName nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                                CityName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                                StadiumDescription nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                                LocativeCapacity int NOT NULL,
                                                CONSTRAINT PK_Stadiums PRIMARY KEY (StadiumName)
);


-- WorldCupPlannerDb.dbo.Teams definition

-- Drop table

-- DROP TABLE WorldCupPlannerDb.dbo.Teams;

CREATE TABLE WorldCupPlannerDb.dbo.Teams (
                                             Name nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                             Confederation nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                             FifaRanking int NOT NULL,
                                             CurrentRanking int NOT NULL,
                                             FlagBase64 nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                             CONSTRAINT PK_Teams PRIMARY KEY (Name)
);


-- WorldCupPlannerDb.dbo.Users definition

-- Drop table

-- DROP TABLE WorldCupPlannerDb.dbo.Users;

CREATE TABLE WorldCupPlannerDb.dbo.Users (
                                             Email nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                             FirstName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                             LastName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                             BirthDate datetime2 NOT NULL,
                                             PasswordHash nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                             Roles nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                             CONSTRAINT PK_Users PRIMARY KEY (Email)
);


-- WorldCupPlannerDb.dbo.[__EFMigrationsHistory] definition

-- Drop table

-- DROP TABLE WorldCupPlannerDb.dbo.[__EFMigrationsHistory];

CREATE TABLE WorldCupPlannerDb.dbo.[__EFMigrationsHistory] (
                                                               MigrationId nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    ProductVersion nvarchar(32) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY (MigrationId)
    );


-- WorldCupPlannerDb.dbo.Groups definition

-- Drop table

-- DROP TABLE WorldCupPlannerDb.dbo.Groups;

CREATE TABLE WorldCupPlannerDb.dbo.Groups (
                                              Id int IDENTITY(1,1) NOT NULL,
                                              GroupLetter nvarchar(1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                              FixtureId int DEFAULT 0 NOT NULL,
                                              CONSTRAINT PK_Groups PRIMARY KEY (Id),
                                              CONSTRAINT FK_Groups_Fixtures_FixtureId FOREIGN KEY (FixtureId) REFERENCES WorldCupPlannerDb.dbo.Fixtures(Id) ON DELETE CASCADE
);
CREATE NONCLUSTERED INDEX IX_Groups_FixtureId ON WorldCupPlannerDb.dbo.Groups (  FixtureId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- WorldCupPlannerDb.dbo.Matches definition

-- Drop table

-- DROP TABLE WorldCupPlannerDb.dbo.Matches;

CREATE TABLE WorldCupPlannerDb.dbo.Matches (
                                               IdMatch int NOT NULL,
                                               DateMatch datetime2 NOT NULL,
                                               StadiumName nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                               HomeTeamName nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                               AwayTeamName nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [Group] nvarchar(1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    IsLocked bit NOT NULL,
    HomeGoals int NULL,
    AwayGoals int NULL,
    WinnerMatchName nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    LoserMatchName nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    CONSTRAINT PK_Matches PRIMARY KEY (IdMatch),
    CONSTRAINT FK_Matches_Stadiums_StadiumName FOREIGN KEY (StadiumName) REFERENCES WorldCupPlannerDb.dbo.Stadiums(StadiumName),
    CONSTRAINT FK_Matches_Teams_AwayTeamName FOREIGN KEY (AwayTeamName) REFERENCES WorldCupPlannerDb.dbo.Teams(Name),
    CONSTRAINT FK_Matches_Teams_HomeTeamName FOREIGN KEY (HomeTeamName) REFERENCES WorldCupPlannerDb.dbo.Teams(Name),
    CONSTRAINT FK_Matches_Teams_LoserMatchName FOREIGN KEY (LoserMatchName) REFERENCES WorldCupPlannerDb.dbo.Teams(Name),
    CONSTRAINT FK_Matches_Teams_WinnerMatchName FOREIGN KEY (WinnerMatchName) REFERENCES WorldCupPlannerDb.dbo.Teams(Name)
    );
CREATE NONCLUSTERED INDEX IX_Matches_AwayTeamName ON WorldCupPlannerDb.dbo.Matches (  AwayTeamName ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Matches_HomeTeamName ON WorldCupPlannerDb.dbo.Matches (  HomeTeamName ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Matches_LoserMatchName ON WorldCupPlannerDb.dbo.Matches (  LoserMatchName ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Matches_StadiumName ON WorldCupPlannerDb.dbo.Matches (  StadiumName ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Matches_WinnerMatchName ON WorldCupPlannerDb.dbo.Matches (  WinnerMatchName ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- WorldCupPlannerDb.dbo.Cruces definition

-- Drop table

-- DROP TABLE WorldCupPlannerDb.dbo.Cruces;

CREATE TABLE WorldCupPlannerDb.dbo.Cruces (
                                              Id int IDENTITY(1,1) NOT NULL,
                                              MatchId int NOT NULL,
                                              Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                              Phase int NOT NULL,
                                              CONSTRAINT PK_Cruces PRIMARY KEY (Id),
                                              CONSTRAINT FK_Cruces_Matches_MatchId FOREIGN KEY (MatchId) REFERENCES WorldCupPlannerDb.dbo.Matches(IdMatch)
);
CREATE NONCLUSTERED INDEX IX_Cruces_MatchId ON WorldCupPlannerDb.dbo.Cruces (  MatchId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- WorldCupPlannerDb.dbo.MatchIncidents definition

-- Drop table

-- DROP TABLE WorldCupPlannerDb.dbo.MatchIncidents;

CREATE TABLE WorldCupPlannerDb.dbo.MatchIncidents (
                                                      Id int IDENTITY(1,1) NOT NULL,
    [Type] nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    TeamName nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    Quantity int NOT NULL,
    MatchId int NOT NULL,
    CONSTRAINT PK_MatchIncidents PRIMARY KEY (Id),
    CONSTRAINT FK_MatchIncidents_Matches_MatchId FOREIGN KEY (MatchId) REFERENCES WorldCupPlannerDb.dbo.Matches(IdMatch) ON DELETE CASCADE,
    CONSTRAINT FK_MatchIncidents_Teams_TeamName FOREIGN KEY (TeamName) REFERENCES WorldCupPlannerDb.dbo.Teams(Name)
    );
CREATE NONCLUSTERED INDEX IX_MatchIncidents_MatchId ON WorldCupPlannerDb.dbo.MatchIncidents (  MatchId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_MatchIncidents_TeamName ON WorldCupPlannerDb.dbo.MatchIncidents (  TeamName ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;