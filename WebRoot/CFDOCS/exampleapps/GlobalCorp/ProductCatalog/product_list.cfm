<!--- ColdFusion(r) Express Global Corp. Example Application --->

<!--- Verify good category_id first.  --->
<CFIF NOT IsDefined("URL.category_id")>
	<CFLOCATION URL="index.cfm" ADDTOKEN="No">
</CFIF>

<!--- Query the database for the name of the category based on the url.category_id The variable application.ds is a global variable naming the datasource used in all of the sample applications.  it is created in the globalvariables.cfm. See application.cfm for its usage --->
<CFQUERY NAME="GetCategory" DATASOURCE="#application.ds#">
SELECT	strCatName
FROM	tblCat
WHERE	strCatID = '#url.category_id#'
</CFQUERY> 
 
<!--- If no category is found, kick the user back to the category list ---> 
<CFIF NOT GetCategory.Recordcount>
	<CFLOCATION URL="index.cfm" ADDTOKEN="No">
</CFIF>

<!--- Query the database for the products associated with the category. Sort by product name --->
<CFQUERY NAME="GetProducts" DATASOURCE="#application.ds#">
	SELECT		strPrdID,
				strPrdName
	FROM		tblProd
	WHERE		strCatIDFK = '#url.category_id#'
	ORDER BY	strPrdName
</CFQUERY>


<!--- Include the Global Header --->
<CFSET header.page_name="Product List: #GetCategory.strCatName#">
<CFINCLUDE TEMPLATE="../CommonFiles/_header.cfm">






<!--- Output through the query to produce a list of products. Wrap each product in a link that passes the product_id to the product detail page. --->
<UL>
<CFOUTPUT QUERY="GetProducts">
	<!--- The function urlencodedformat() is used to ensure no improper characters are appended to the url.  this function should be  used when putting dynamic information in a url. --->
	<LI><A HREF="product_detail.cfm?product_id=#urlencodedformat(strPrdID)#">#strPrdName#</A>
</CFOUTPUT>
</UL>


[ <A HREF="index.cfm">Back to Category List</A> ]



<!--- Include the Global Footer --->
<CFINCLUDE TEMPLATE="../CommonFiles/_footer.cfm">

