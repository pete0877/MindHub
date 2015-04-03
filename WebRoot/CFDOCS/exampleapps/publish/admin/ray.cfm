<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
	<title>Untitled</title>
</head>

<body>

<CFQUERY NAME="GetAll" datasource="globalcorpdb">
SELECT a.strattndid FROM tblAttnd a
where a.strattndid ='5'
</cfquery>

<CFOUTPUT></CFOUTPUT>
</body>
</html>
