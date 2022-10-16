CREATE PROCEDURE dbo.TournamentEntries_Insert
	@TournamendID int,
	@TeamID int,
	@id int = 0 output
AS
BEGIN
	SET NOCOUNT ON;

	insert into dbo.TournamentEntries(TournamentID, TeamID)
	values (@TournamendID, @TeamID);

	select @id = SCOPE_IDENTITY(); 
END
GO
