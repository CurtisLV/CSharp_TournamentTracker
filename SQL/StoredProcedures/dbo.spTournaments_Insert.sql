CREATE PROCEDURE dbo.spTournaments_Insert

@TournamentName nvarchar(200),
@EntryFee money,
@id int = 0 output

AS
BEGIN

	SET NOCOUNT ON;
	
	insert into dbo.Tournaments(TournamentName, EntryFee, Active)
	values (@TournamentName, @EntryFee, 1);
	
	select @id = SCOPE_IDENTITY();
END
GO
