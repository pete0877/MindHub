<!--- This is the default viewer for CF_ShowContent. --->

<!--- All object types' teasers look pretty much the
	same... so we start with the same two items --->
<CFOUTPUT><P>
<DIV CLASS="HeadlineTeaser">#CurrObject.Headline#<BR></DIV>
<DIV CLASS="TeaserTeaser">#CurrObject.Teaser#<BR></DIV>

<!--- Now we need to make a decision based on what kind 
	of object we're dealing with. --->

<!--- If we're dealing with plain text... --->
<CFIF CurrObject.ClassName IS 'News Item'>

	<DIV CLASS="LinkTeaser"><A HREF="viewfull.cfm?ObjectID=#CurrObject.ObjectID#">More Info</A></DIV>

<!--- If we're dealing with a binary object... --->
<CFELSEIF CurrObject.ClassName IS 'File with Description'>

	<DIV CLASS="LinkTeaser"><A HREF="binarydata/#Replace(Replace(URLEncodedFormat(CurrObject.File),"%2E",".","ALL"),"+","%20","ALL")#">Download File</A></DIV>

<!--- If we're dealing with a hyperlink... --->
<CFELSEIF CurrObject.ClassName IS 'Hyperlink'>

	<DIV CLASS="LinkTeaser"><A HREF="#CurrObject.HREF#">Go to page</A></DIV>

</CFIF>

<!--- If this page is being browsed in administrator
	mode, little icons should be displayed which will
    allow the user to edit/delete this object. --->
<CFIF IsDefined("Cookie.PubAdminMode")>
	<A HREF="admin/properties.cfm?ObjectID=#strObjID#"><IMG SRC="open.gif" WIDTH=16 HEIGHT=14 BORDER=0 ALT="Open" ALIGN="TOP"></A> <A HREF="admin/deleteinstance.cfm?InstanceID=#strInsID#" onClick="return confirm('This will delete this instance (the object will remain intact).\n\nSure you want to continue?')"><IMG SRC="delete.gif" WIDTH=15 HEIGHT=16 BORDER=0 ALT="Delete" ALIGN="TOP"></A>
</CFIF>

</CFOUTPUT>