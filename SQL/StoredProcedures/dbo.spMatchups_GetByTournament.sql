CREATE PROCEDURE dbo.spMatchups_GetByTournament
	@TournamentId int
AS
BEGIN
	SET NOCOUNT ON;

	select m.*
	from dbo.Matchups m
	where m.TournamentId = @TournamentId;
END
GO