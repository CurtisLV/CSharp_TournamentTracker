CREATE PROCEDURE dbo.spTournamentPrizes_Insert

	@TournamentID int,
	@PrizeID int,
	@id int = 0 output
AS
BEGIN
	SET NOCOUNT ON;

	insert into dbo.TournamentPrizes(TournamentID, PrizeID)
	values (@TournamentID, @PrizeID);

	select @id = SCOPE_IDENTITY();

END
GO
