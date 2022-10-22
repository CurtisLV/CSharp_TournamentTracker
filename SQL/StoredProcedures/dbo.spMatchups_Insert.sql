CREATE PROCEDURE dbo.spMatchups_Insert
	@MatchupRound int,
	@TournamentId int,
	@id int = 0 output
AS
BEGIN
	SET NOCOUNT ON;

	insert into dbo.Matchups(TournamentId, MatchupRound)
	values (@TournamentId, @MatchupRound);
	SELECT @id = SCOPE_IDENTITY();
END
GO
       