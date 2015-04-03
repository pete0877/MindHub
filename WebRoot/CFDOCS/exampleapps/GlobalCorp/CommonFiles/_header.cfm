<!--- ColdFusion(r) Express Global Corp. Example Application --->



<HTML>

<CFOUTPUT>

<HEAD>

	<TITLE>

	<!--- If a variable called "header.page_name" exists, include it in the title --->

	Global Corp<CFIF IsDefined("header.page_name")>: #header.page_name#</CFIF>

	</TITLE>

	<!--- Global CSS --->

	<STYLE TYPE="text/css">

	BODY {font-family: #application.font#;}

	P {font-family: #application.font#;}

	TABLE, TR, TD {font-family: #application.font#;}

	ALL.Menu {font-family: #application.font#; font-size: small;}

	</STYLE>	 

	<META HTTP-EQUIV="Expires" CONTENT="Mon, 06 Jan 1990 00:00:01 GMT">

</HEAD>

</CFOUTPUT>



<!--- Notice the use of global variables in the "application" scope. these variables are created in the application.cfm file --->



<CFOUTPUT>


<BODY BGCOLOR="#application.bgcolor#" LINK="#application.link_color#" VLINK="#application.followed_link_color#" ALINK="#application.anchor_link_color#" TEXT="#application.font_color#" TOPMARGIN="0" MARGINHEIGHT="0">

<A NAME="Top"></A>

<CENTER>

<TABLE CELLSPACING="0" CELLPADDING="0" BORDER="0" WIDTH="583">

<TR>

    <TD ROWSPAN="2"><A HREF="../#application.root_path#index.cfm"><IMG SRC="../#application.image_path#header_logo.jpg" WIDTH="69" HEIGHT="40" BORDER="0" ALT="Return to Home Page"></A></TD>

    <TD COLSPAN="8"><IMG SRC="../#application.image_path#header_top.jpg" WIDTH="514" HEIGHT="24" BORDER="0"></TD>

</TR>

<TR>

    <TD><A HREF="../#application.root_path#EmployeeDirectory/index.cfm"><IMG SRC="../#application.image_path#header_button_address.jpg" WIDTH="127" HEIGHT="16" BORDER="0"></A></TD>

    <TD><IMG SRC="../#application.image_path#header_spacer.jpg" WIDTH="19" HEIGHT="16" BORDER="0"></TD>

    <TD><A HREF="../#application.root_path#MeetingRoomScheduler/index.cfm"><IMG SRC="../#application.image_path#header_button_meeting.jpg" WIDTH="130" HEIGHT="16" BORDER="0"></A></TD>

    <TD><IMG SRC="../#application.image_path#header_spacer.jpg" WIDTH="19" HEIGHT="16" BORDER="0"></TD>

    <TD><A HREF="../#application.root_path#DiscussionGroup/index.cfm"><IMG SRC="../#application.image_path#header_button_discussion.jpg" WIDTH="101" HEIGHT="16" BORDER="0"></A></TD>

    <TD><IMG SRC="../#application.image_path#header_spacer.jpg" WIDTH="19" HEIGHT="16" BORDER="0"></TD>

    <TD><A HREF="../#application.root_path#ProductCatalog/index.cfm"><IMG SRC="../#application.image_path#header_button_product.jpg" WIDTH="87" HEIGHT="16" BORDER="0"></A></TD>

    <TD><IMG SRC="../#application.image_path#header_spacer_end.jpg" WIDTH="12" HEIGHT="16" BORDER="0"></TD>

</TR>

<TR>

<TD COLSPAN="9" BGCOLOR="#application.white#" ALIGN="CENTER">

<TABLE BORDER="0" CELLPADDING="20" CELLSPACING="0" BGCOLOR="#application.white#" WIDTH="583">

<TR>

<TD>

<!--- If a variable called "header.page_name" exists, display it on the page --->

<H1>#header.page_name#</H1>

</CFOUTPUT>



