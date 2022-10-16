CREATE PROCEDURE dbo.spTournaments_Insert

@TournamentName nvarchar(200),
@EntryFee money,
@Active bit

AS
BEGIN

	SET NOCOUNT ON;
	
	insert into dbo.Tournaments(TournamentName, EntryFee, Active)
	values (@TournamentName, @EntryFee, 1);

END
GO
