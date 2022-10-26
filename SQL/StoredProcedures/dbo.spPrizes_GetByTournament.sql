CREATE PROCEDURE dbo.spPrizes_GetByTournament
	@TournamentId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT p.*
	from dbo.Prizes p
	inner join dbo.TournamentPrizes t on p.id = t.PrizeID
	where t.TournamentID = @TournamentId;
END
GO
