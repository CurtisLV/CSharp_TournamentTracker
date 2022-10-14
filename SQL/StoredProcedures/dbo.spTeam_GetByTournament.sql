USE [Tournaments]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE dbo.spTeam_GetByTournament
	@TournamentId int

AS
BEGIN

	SET NOCOUNT ON;

	select t.* 
	from dbo.Teams t
	inner join dbo.TournamentEntries e on t.id = e.TeamID
	where e.TournamentID = @TournamentId;
END
GO