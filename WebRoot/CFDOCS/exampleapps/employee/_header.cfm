<!--- If there is no variable GetDepartmentName.DepartmentName,
	CFPARAM creates it and assigns the value "NEW LINE SOFTWARE" --->
<CFPARAM NAME="GetDepartmentName.strDptName" DEFAULT="NEW LINE SOFTWARE">

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE><CFOUTPUT>#GetDepartmentName.strDptName#</CFOUTPUT></TITLE>
</HEAD>

<BODY BGCOLOR="#000033" TEXT="#CCCCCC" LINK="#FFFFFF" VLINK="#FFFFFF">

<TABLE BORDER="0" CELLSPACING="0" CELLPADDING="0" ALIGN="CENTER" BGCOLOR="#000066" WIDTH=620>
	<TR>
		<TD WIDTH="25" NOWRAP ROWSPAN="99">&nbsp;</TD>
		<TD COLSPAN="3">&nbsp;</TD>
		<TD WIDTH="25" NOWRAP ROWSPAN="99">&nbsp;</TD>
	</TR>
	<TR>
		<TD COLSPAN="3" VALIGN="top" HEIGHT="75">
			<FONT FACE="Myriad Web, Verdana, Helvetica" SIZE="+2">
				<A HREF="index.cfm"><IMG SRC="images/up.gif" WIDTH=25 HEIGHT=25 BORDER=0 ALIGN="right" ALT="HOME"></A>
				<!--- Displays department name --->
				<CFOUTPUT>#GetDepartmentName.strDptName#</CFOUTPUT>
                <BR><IMG SRC="images/white.gif" HEIGHT=1 WIDTH="569"><BR>
			</FONT>
		</TD>
	</TR>