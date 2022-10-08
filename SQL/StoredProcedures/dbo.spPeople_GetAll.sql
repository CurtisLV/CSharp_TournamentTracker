CREATE PROCEDURE dbo.spPeople_GetAll
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Insert statements for procedure here
	SELECT People.id
		,People.FirstName
		,People.LastName
		,People.EmailAddress
		,People.CellPhoneNumber
	FROM dbo.People
END
GO

