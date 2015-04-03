<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<HTML>

<HEAD>



<TITLE>ColdFusion Example Applications</TITLE>



<META content="text/html; charset=windows-1252" http-equiv="Content-Type">

<META content="MSHTML 5.00.2314.1000" name="GENERATOR">



<SCRIPT language="Javascript">

<!--

var DocRemote = 0;



function makeRemote(){



	if(DocRemote){

		if(DocRemote.closed){

			DocRemote = 0;

			makeRemote();

		}else{

			DocRemote.focus();

		}

	}else{

		DocRemote = window.open('search.cfm', 

					    'Search', 

					    'scrollbars,resizable,width=510,height=300');

    	}

}

//-->

</SCRIPT>



<script language="JavaScript1.1">

<!--

	blank = new Image();

	blank.src = 'images/mouseovers/blank.gif';

	

	imgBack = new Image();

	imgBack.src = 'images/mouseovers/back.gif';



	imgSearch = new Image();

	imgSearch.src = 'images/mouseovers/search.gif';



	imgTop = new Image();

	imgTop.src = 'images/mouseovers/toplevel.gif';



	imgUp = new Image();

	imgUp.src = 'images/mouseovers/uplevel.gif';



	imgNext = new Image();

	imgNext.src = 'images/mouseovers/next.gif';

//-->

</script>



<script language="JavaScript">

<!-- Hide JavaScript



   if (navigator.appName == "Netscape"){

      document.write('<LINK REL=STYLESHEET HREF="allaire_ns.css" TYPE="text/css">');

      }

   else{

      document.write('<LINK REL=STYLESHEET HREF="allaire_ie.css" TYPE="text/css">');

   }

//-->

</script>



</head>

<body>

<!-- Start navigation table -->

<cfset templatePath=getBaseTemplatePath()>

<cfset exampleApps=Replace(#templatePath#, "examplehome.cfm", "snippets")>

<cfif not DirectoryExists(exampleApps)>

	<CFINCLUDE template="/cfdocs/sorry.htm">

	<CFABORT>

</cfif> 



    <table border="0" cellspacing="0" cellpadding="5" align="right">

	<tr>



		<td>

			<img src="images/mouseovers/blank.gif" name="DescText" border=0><br>

		</td>



		<td>

			<a href="index.htm" OnMouseOut="DescText.src=blank.src"

				OnMouseOver="DescText.src=imgBack.src">

			<img src="images/icons/back.gif" border=0></a><br>

		</td>



		<td>

			<a href="index.htm">

			<img src="images/icons/uplevel.gif" border=0 OnMouseOut="DescText.src=blank.src"

			OnMouseOver="DescText.src=imgUp.src"></a><br>

		</td>

		

		<td>

			<A HREF="examplehome.cfm">

			<img src="images/icons/next.gif" border=0 OnMouseOut="DescText.src=blank.src"

			OnMouseOver="DescText.src=imgNext.src"></a><br>

		</td>



		<td>

			<a href="index.htm">

			<img src="images/icons/home.gif" border=0 OnMouseOut="DescText.src=blank.src"

			OnMouseOver="DescText.src=imgTop.src"></a><br>

		</td>

		

		<td>

			<a href="javascript: makeRemote();" OnMouseOut="DescText.src=blank.src"

			OnMouseOver="DescText.src=imgSearch.src">

			<img src="images/icons/search.gif" border=0></a><br>

		</td>



	</tr>

	</table><br><br><br><br>

<!-- End navigation table -->





<hr>



<A name="top"></A>

<h1>ColdFusion Example Applications</H1>



<p>ColdFusion example applications are a tremendous learning resource for ColdFusion developers. These examples

are complete and functional applications you can adapt and use as you like. Each application demonstrates a particular set of techniques you can observe and adapt to your own purposes.  </p>



<h4><a href="exampleapps/GlobalCorp/index.cfm" target="_blank" title="A small intranet example">Global Corp. Intranet</A></H4>





<table cellspacing=10>

<tr>



<td valign="top"><a href="exampleapps/GlobalCorp/index.cfm" target="_blank"><IMG SRC="exampleapps/global_thumbnail.gif" WIDTH=148 HEIGHT=104 ALT="Global Corp. intranet" BORDER="0"></A></TD>



<td valign="top" width=350>





<p>The Global Corp. suite of intranet applications illustrates how much you can do with just a few CFML tags. Start with a tour of these applications for an introduction to ColdFusion programming basics and the RAD (Rapid Application Development) way of life.  </p>



</TD>

</TR>



</TABLE>









<h4><a href="exampleapps/employee/index.cfm" title="Employee management example" target="_blank">New Line Employee Management</a></H4>







<table cellspacing=10>

<tr>



<td valign="top"><a href="exampleapps/employee/index.cfm" target="_blank"><IMG SRC="exampleapps/emp_thumbnail.gif" WIDTH=146 HEIGHT=95 ALT="New Line Software" BORDER="0"></a></TD>



<td valign="top" width=350>



<p>New Line is a very easy to understand employee management application. Check out this applet for examples of fundamental ColdFusion practices.</p>

</TD>

</TR>



</TABLE>











<h4><a href="exampleapps/email/login.cfm" TARGET="_blank" title="Basic POP mail client example">Crazy Cab Email Client</a></H4>



<table cellspacing=10>

<tr>



<td valign="top"><a href="exampleapps/email/login.cfm" TARGET="_blank"><IMG SRC="exampleapps/email_thumbnail.gif" WIDTH=131 HEIGHT=136 ALT="Crazy Cab email" BORDER="0"></a></TD>

<td valign="top" width=350>



<p>Crazy Cab is a (practically) fully functional (some would say superior) email client application that demonstrates a number of more advanced ColdFusion techniques. If you're just getting started with ColdFusion, take a look at the New Line or Global Corp. example applications first, then nose around in Crazy Cab for greater depth. </p>



</TD>

</TR>



</TABLE>







<h4><a href="exampleapps/store/showcategories.cfm" TARGET="_blank" title="Online store example">Tack2 Online Store</a></H4>



<table cellspacing=10>

<tr>



<td valign="top"><a href="exampleapps/store/showcategories.cfm" TARGET="_blank"><IMG SRC="exampleapps/store_thumbnail.gif" WIDTH=109 HEIGHT=128 ALT="Tack2 Online store" BORDER="0"></a></TD>

<td valign="top" width=350>









<p>Tack2 is a working online storefront, offering (okay, fictitious) windsurfing hardware. Check out the interactive, behind-the-scenes feature for viewing the CFML code at work in this application.</p>



</TD>

</TR>



</TABLE>











<h4><a href="exampleapps/publish/welcome.cfm" TARGET="_blank" title="Content management example">Tack2 Intranet</a></H4>



<table cellspacing=10>

<tr>



<td valign="top"><a href="exampleapps/publish/welcome.cfm" TARGET="_blank"><IMG SRC="exampleapps/publish_thumbnail.gif" WIDTH=115 HEIGHT=153 ALT="Tack2 Intranet" BORDER="0"></a></TD>

<td valign="top" width=350>









<p>The Tack2 Intranet is an example of how ColdFusion can be used to build content

management functionality into a corporate Intranet. But before you venture into the 

this example app, please check out the 

<a href="exampleapps/docs/pubdocs/overview.cfm" TARGET="_blank"> Tack2 

Overview</a>, which explains how to use the Tack2 content management functionality. 

This relatively complex example offers a scaled down version of the same 

sophisticated content management functionality we use on our own Web site. </p>





</TD>

</TR>



</TABLE>







</body>

</html>

