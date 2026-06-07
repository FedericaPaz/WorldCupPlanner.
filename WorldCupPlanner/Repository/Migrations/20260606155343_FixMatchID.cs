using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    public partial class FixMatchID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
    CREATE TABLE [Matches_New] (
        [IdMatch] int NOT NULL,
        [AwayGoals] int NULL,
        [AwayTeamName] nvarchar(450) NULL,
        [DateMatch] datetime2 NOT NULL,
        [Group] nvarchar(1) NOT NULL,
        [HomeGoals] int NULL,
        [HomeTeamName] nvarchar(450) NULL,
        [IsLocked] bit NOT NULL,
        [LoserMatchName] nvarchar(450) NULL,
        [StadiumName] nvarchar(450) NULL,
        [WinnerMatchName] nvarchar(450) NULL,
        CONSTRAINT [PK_Matches_New] PRIMARY KEY ([IdMatch])
    );

    INSERT INTO [Matches_New] (
        [IdMatch], [AwayGoals], [AwayTeamName], [DateMatch], [Group],
        [HomeGoals], [HomeTeamName], [IsLocked], [LoserMatchName],
        [StadiumName], [WinnerMatchName]
    )
    SELECT
        [IdMatch], [AwayGoals], [AwayTeamName], [DateMatch], [Group],
        [HomeGoals], [HomeTeamName], [IsLocked], [LoserMatchName],
        [StadiumName], [WinnerMatchName]
    FROM [Matches];

    ALTER TABLE [MatchIncidents] DROP CONSTRAINT [FK_MatchIncidents_Matches_MatchId];
    ALTER TABLE [Cruces] DROP CONSTRAINT [FK_Cruces_Matches_MatchId];

    DROP TABLE [Matches];
    EXEC sp_rename 'Matches_New', 'Matches';
    EXEC sp_rename 'PK_Matches_New', 'PK_Matches', 'OBJECT';

    ALTER TABLE [MatchIncidents] ADD CONSTRAINT [FK_MatchIncidents_Matches_MatchId]
        FOREIGN KEY ([MatchId]) REFERENCES [Matches] ([IdMatch]) ON DELETE CASCADE;
    ALTER TABLE [Cruces] ADD CONSTRAINT [FK_Cruces_Matches_MatchId]
        FOREIGN KEY ([MatchId]) REFERENCES [Matches] ([IdMatch]) ON DELETE CASCADE;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE [Matches_New] (
                    [IdMatch] int NOT NULL IDENTITY(1,1),
                    [AwayGoals] int NULL,
                    [AwayTeamName] nvarchar(450) NULL,
                    [DateMatch] datetime2 NOT NULL,
                    [Group] nvarchar(1) NOT NULL,
                    [HomeGoals] int NULL,
                    [HomeTeamName] nvarchar(450) NULL,
                    [IsLocked] bit NOT NULL,
                    [LoserMatchName] nvarchar(450) NULL,
                    [StadiumName] nvarchar(450) NULL,
                    [WinnerMatchName] nvarchar(450) NULL,
                    CONSTRAINT [PK_Matches_New] PRIMARY KEY ([IdMatch])
                );

                ALTER TABLE [MatchIncidents] DROP CONSTRAINT [FK_MatchIncidents_Matches_MatchId];

                DROP TABLE [Matches];
                EXEC sp_rename 'Matches_New', 'Matches';
                EXEC sp_rename 'PK_Matches_New', 'PK_Matches', 'OBJECT';

                ALTER TABLE [MatchIncidents] ADD CONSTRAINT [FK_MatchIncidents_Matches_MatchId]
                    FOREIGN KEY ([MatchId]) REFERENCES [Matches] ([IdMatch]) ON DELETE CASCADE;
            ");
        }
    }
}