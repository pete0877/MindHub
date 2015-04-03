<!--- ColdFusion(r) Express Global Corp. Example Application --->


<!--- Query the database for the categories, sort by name.  The variable application.ds is a global variable naming the datasource used in all of the sample applications --->
<CFQUERY NAME="GetCategories" DATASOURCE="#application.ds#">
	SELECT		strCatName,
				strCatID,
				strPrdName,
				strPrdID
	FROM		tblCat,
				tblProd
	WHERE		strCatID = strCatIDFK
	ORDER BY 	strCatName
</CFQUERY> 


<!--- Include the Global Header --->
<CFSET header.page_name="Product Categories">
<CFINCLUDE TEMPLATE="../CommonFiles/_header.cfm">




<!--- Output through the query to produce a list of categories and products, grouped by category.  notice the use of nested cfoutput with the first cfoutput using the group attribute --->
<UL>
<CFOUTPUT QUERY="GetCategories" GROUP="strCatID">
	<LI><B>#strCatname#</B>
	<UL>
	<CFOUTPUT>
		<!--- The function UrlEncodedFormat() is used to ensure no improper characters are appended to the url.  this function should be  used when putting dynamic information in a url. --->
		<LI><A HREF="product_detail.cfm?product_id=#UrlEncodedFormat(strPrdID)#">#strPrdname#</A>
	</CFOUTPUT>
	</UL>

</CFOUTPUT>
</UL>



<!--- Include the Global Footer --->
<CFINCLUDE TEMPLATE="../CommonFiles/_footer.cfm">



