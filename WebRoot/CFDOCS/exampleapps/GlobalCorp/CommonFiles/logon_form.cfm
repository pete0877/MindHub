<!--- ColdFusion(r) Express Global Corp. Example Application --->



<HTML>

<HEAD>

	<TITLE>Global Corp. Log In Form</TITLE>

		

	<CFOUTPUT>

	<!--- Global CSS --->

	<STYLE TYPE="text/css">

	BODY {font-family: #application.font#;}

	P {font-family: #application.font#;}

	TABLE, TR, TD {font-family: #application.font#;}

	</STYLE>	

	</CFOUTPUT>

	<META HTTP-EQUIV="Expires" CONTENT="Mon, 06 Jan 1990 00:00:01 GMT">

</HEAD>



<!--- Get a list of employees from the database. --->

<CFQUERY NAME="GetEmployees" DATASOURCE="#application.ds#">

	SELECT		strEmpID,

				strFName,

				strLName

	FROM 		tblEmp

	ORDER BY 	strLName

</CFQUERY>



<!--- Use global variables set in application.cfm for body attributes --->

<CFOUTPUT>

<BODY BGCOLOR="#application.DarkOrange#" MARGINHEIGHT="0" TOPMARGIN="0" MARGINWIDTH="0" LEFTMARGIN="0">



<!--- Notice the double ## in the bgcolor of the table to escape it within cfoutput  --->

<TABLE WIDTH="701" BORDER="0" CELLPADDING="0" CELLSPACING="0" BGCOLOR="#application.white#">

	<TR>

		<TD WIDTH="701" HEIGHT="184" COLSPAN="2">

			<IMG SRC="#application.image_path#home_top.jpg" WIDTH="701" HEIGHT="184" BORDER="0">

		</TD>

	</TR>







	<TR>

		<TD HEIGHT="119" WIDTH="76">

			<IMG SRC="#application.image_path#space.jpg" WIDTH="76" HEIGHT="119" BORDER="0">

		</TD>

		<TD>

		<P>

		

		

		

		<!--- Submit the page to whatever page you are on.  This allows a user to bookmark a page and then log into it later --->



		<FORM ACTION="#getfilefrompath(getbasetemplatepath())#" METHOD="POST">



		

		

		<INPUT TYPE="hidden" NAME="LoggingOn" VALUE="1">

		<B>Welcome to Global Corp.  </B>

		<BR>

		<!--- If the variable form.loggingon exists it means the user hit the form, but did not get logged in.  --->

		<CFIF IsDefined("Form.LoggingOn")>

			<FONT COLOR="##FF0000">That is not a valid password.  Please try again.</FONT>

		<CFELSE>

			Please select your name from the list and enter your password.

		</CFIF>

		

		<BR>

		(all passwords are "cfexpress")

		<P>

		<SELECT NAME="Employee">

		

</CFOUTPUT>		

		<!--- Display every employee. --->

		<CFOUTPUT QUERY="GetEmployees">

			<OPTION VALUE="#strEmpID#">#strFName# #strLName#</OPTION>

		</CFOUTPUT>

		

		<!--- field for the password --->

		</SELECT>

		<B>Password:</B>

		<INPUT TYPE="password" NAME="Password" SIZE="15">

		<INPUT TYPE="Submit" VALUE="Logon">

		</FORM>

		<FONT SIZE="1">Global Corp. is a fictitious company created to demonstrate ColdFusion Express functionality.</FONT>

		</TD>

</TR>

<CFOUTPUT>

	<TR>

		<TD WIDTH="701" HEIGHT="69" COLSPAN="2">

		<IMG SRC="#application.image_path#home_bottom.jpg" WIDTH="701" HEIGHT="69" BORDER="0"></TD></TR>

</TABLE>



</CFOUTPUT>

 

 









</BODY>

</HTML>





