CREATE PROCEDURE [dbo].[GetDecryptValue] 
	-- Add the parameters for the stored procedure here
	@EncryptText varbinary(MAX) = null, 
	@DecryptText varchar(Max) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	OPEN SYMMETRIC KEY ProjectTemplateSymKey DECRYPTION BY CERTIFICATE ProjectTemplateCerti

	Select @DecryptText = CONVERT(VARCHAR(MAX), DECRYPTBYKEY(@EncryptText))

	CLOSE SYMMETRIC KEY ProjectTemplateSymKey
END

GO

CREATE PROCEDURE [dbo].[CheckUserExist]
	@Email varchar(max)=NULL,
	@Id int=0,
	@EmailCount varchar(max) OUTPUT
AS
BEGIN
	DECLARE @WhereClause VARCHAR(MAX)=' AND IsDelete=0'
	DECLARE @SQL NVARCHAR(MAX)=''

	If @Id != 0
	BEGIN
		SET @WhereClause = @WhereClause + ' And Id !='+ CONVERT(VARCHAR,@Id)
	END

	If @Email != '' and @Email is not null
	BEGIN
		SET @WhereClause = @WhereClause + ' And CONVERT(VARCHAR(MAX), DECRYPTBYKEY(encEmail)) ='''+ CONVERT(VARCHAR,@Email)+''''
	END
	
	OPEN SYMMETRIC KEY ProjectTemplateSymKey DECRYPTION BY CERTIFICATE ProjectTemplateCerti

	SET @SQL = 'SELECT @EmailCount=count(*) FROM [UserInfo]  where 1=1 '+@WhereClause
	Print @SQL
	EXECUTE sp_executesql @SQL,N'@EmailCount VARCHAR(MAX) out',@EmailCount OUT
	
	CLOSE SYMMETRIC KEY ProjectTemplateSymKey
End

