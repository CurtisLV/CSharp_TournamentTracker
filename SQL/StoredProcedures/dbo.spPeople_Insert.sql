-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE dbo.spPeople_Insert
	@FirstName nvarchar(100),
	@LastName nvarchar(100),
	@EmailAddress nvarchar(100),
	@CellPhoneNumber varchar(20),
	@Id int = 0 output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	insert into dbo.People (FirstName, LastName, EmailAddress, CellPhoneNumber)
	values (@FirstName, @LastName, @EmailAddress, @CellPhoneNumber);

	select @id=SCOPE_IDENTITY();
END
GO
