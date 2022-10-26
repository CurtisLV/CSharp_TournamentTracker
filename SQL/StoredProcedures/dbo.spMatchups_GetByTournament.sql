CREATE PROCEDURE dbo.spMatchups_GetByTournament
	@TournamentId int
AS
BEGIN
	SET NOCOUNT ON;

	select m.*, t.TeamName
	from dbo.Matchups m
	left join dbo.Teams t on m.WinnerId = t.id
	where m.TournamentId = @TournamentId;
END
GO