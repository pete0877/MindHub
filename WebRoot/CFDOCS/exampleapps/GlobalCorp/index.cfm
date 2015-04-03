<!--- ColdFusion(r) Express Global Corp. Example Application --->



<HTML>

<HEAD>

<TITLE>Global Corp. Home Page</TITLE>

	<!--- Use global variables set in application.cfm for style attributes --->

	<CFOUTPUT>

	<!--- Global CSS --->

	<STYLE TYPE="text/css">

	BODY {font-family: #application.font#;}

	P {font-family: #application.font#;}

	TABLE, TR, TD {font-family: #application.font#;}

	</STYLE>		

	

	

	<!--- The javascript for the roll-over of the descriptions. We use application.image_path, which is set in the application.cfm file. This is a global variable for all images on the site.--->

	<SCRIPT LANGUAGE="JavaScript1.1">

		//If this browser recognizes the document.images object, make the images to use for descriptions

		if (document.images){

			DescriptionDefault = new Image(390,120); 

			DescriptionDefault.src = '#application.image_path#home_description_default.jpg';       

			

			DescriptionAddress = new Image(390,120);

			DescriptionAddress.src = '#application.image_path#home_description_address.jpg';

			

			DescriptionDiscussion = new Image(390,120);

			DescriptionDiscussion.src = '#application.image_path#home_description_discussion.jpg';

			

			DescriptionMeeting = new Image(390,120);

			DescriptionMeeting.src = '#application.image_path#home_description_meeting.jpg';

			

			DescriptionProduct = new Image(390,120);

			DescriptionProduct.src = '#application.image_path#home_description_product.jpg';

		}

		//If this browser recognizes the document.images object, flip the description image based on what is passed in

		function ShowDescription(TheImage){

			if (document.images){

				document.images.DescriptionImage.src = TheImage.src;

			}

		}

	</SCRIPT>

	</CFOUTPUT>

</HEAD>



<!--- Use global variables set in application.cfm for body attributes --->

<CFOUTPUT>

<CFPARAM name="LOGOUT" default="NO">

<CFIF LOGOUT>

	<CFABORT>

</cfif>

<BODY BGCOLOR="#application.bgcolor#" MARGINHEIGHT="0" TOPMARGIN="0" MARGINWIDTH="0" LEFTMARGIN="0">



<TABLE WIDTH="701" BORDER="0" CELLPADDING="0" CELLSPACING="0" BGCOLOR="#application.white#">

	<TR>

		<!--- We use application.image_path, which is set in the application.cfm file. This is a global variable for all images on the site. --->

		<TD WIDTH="701" HEIGHT="184" COLSPAN="5">
			

			
			<IMG SRC="#application.image_path#home_top.jpg" WIDTH="701" HEIGHT="184" BORDER="0">

		</TD>

	</TR>

	<TR>		

		<TD HEIGHT="120" WIDTH="76">

			<IMG SRC="#application.image_path#space.jpg" WIDTH="76" HEIGHT="120" BORDER="0">

		</TD>

		<!--- The navigation buttons, with roll over effect --->

		<TD HEIGHT="120" WIDTH="180">

			<A HREF="#application.root_path#EmployeeDirectory/index.cfm" ONMOUSEOVER="ShowDescription(DescriptionAddress);" ONMOUSEOUT="ShowDescription(DescriptionDefault);"><IMG SRC="#application.image_path#home_button_address.jpg" WIDTH="180" HEIGHT="20" BORDER="0"></A>

			<A HREF="#application.root_path#DiscussionGroup/index.cfm" ONMOUSEOVER="ShowDescription(DescriptionDiscussion);" ONMOUSEOUT="ShowDescription(DescriptionDefault);"><IMG SRC="#application.image_path#home_button_discussion.jpg" WIDTH="180" HEIGHT="20" BORDER="0"></A>

			<A HREF="#application.root_path#MeetingRoomScheduler/index.cfm" ONMOUSEOVER="ShowDescription(DescriptionMeeting);" ONMOUSEOUT="ShowDescription(DescriptionDefault);"><IMG SRC="#application.image_path#home_button_meeting.jpg" WIDTH="180" HEIGHT="20" BORDER="0"></A>

			<A HREF="#application.root_path#ProductCatalog/index.cfm" ONMOUSEOVER="ShowDescription(DescriptionProduct);" ONMOUSEOUT="ShowDescription(DescriptionDefault);"><IMG SRC="#application.image_path#home_button_product.jpg" WIDTH="180" HEIGHT="20" BORDER="0"></A>

			<A HREF="#application.root_path#About/index.cfm"><IMG SRC="#application.image_path#home_button_about.jpg" WIDTH="180" HEIGHT="20" BORDER="0"></A>

			<A HREF="#application.root_path#index.cfm?logout=yes"><IMG SRC="#application.image_path#home_button_logout.jpg" WIDTH="180" HEIGHT="20" BORDER="0"></A>

		</TD>

		<TD HEIGHT="120" WIDTH="25">

			<IMG SRC="#application.image_path#space.jpg" WIDTH="25" HEIGHT="120" BORDER="0">

		</TD>		

		<!--- The description image that changes with roll overs above --->

		<TD HEIGHT="120" WIDTH="390">

			<IMG SRC="#application.image_path#home_description_default.jpg" WIDTH="390" HEIGHT="120" BORDER="0" NAME="DescriptionImage">

		</TD>

		<TD HEIGHT="120" WIDTH="30">

			<IMG SRC="#application.image_path#space.jpg" WIDTH="30" HEIGHT="120" BORDER="0">

		</TD>

	</TR>

	<TR>

		<TD WIDTH="701" HEIGHT="69" COLSPAN="5">

		<IMG SRC="#application.image_path#home_bottom.jpg" WIDTH="701" HEIGHT="69" BORDER="0"></TD>

	</TR>

</TABLE>



</CFOUTPUT>

 

 





</BODY>

</HTML>



