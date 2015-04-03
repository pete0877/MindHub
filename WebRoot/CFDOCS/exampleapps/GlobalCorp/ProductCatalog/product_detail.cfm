<!--- ColdFusion(r) Express Global Corp. Example Application --->

<!--- 	Verify good product_id first.  --->
<CFIF NOT IsDefined("URL.product_id")>
	<CFLOCATION URL="index.cfm" ADDTOKEN="No">
</CFIF>

<!---  Query the database for the product information based on the url.product_id.  The variable application.ds is a global variable naming the datasource used in all of the sample applications.  It is created in the globalvariables.cfm. see application.cfm for its usage --->
<CFQUERY NAME="GetProduct" DATASOURCE="#application.ds#">
	SELECT	strPrdName,
			dblPrdCost,
			glbPrdDesc,
			strCatIDFK
	FROM	tblProd
	WHERE	strPrdID = '#url.product_id#'
</CFQUERY> 
 
<!--- If no product is found, give the user a message and a link back to the category list ---> 

<!--- If no category is found, kick the user back to the category list ---> 
<CFIF NOT GetProduct.Recordcount>
	<CFLOCATION URL="index.cfm" ADDTOKEN="No">
</CFIF>






<!--- Output using the query to produce information about the product. --->


<CFOUTPUT QUERY="GetProduct">

<!--- Include the Global Header --->
<CFSET header.page_name="Product Detail: #strPrdName#">
<CFINCLUDE TEMPLATE="../CommonFiles/_header.cfm">

<!--- Product Catalog Menu --->
<FONT SIZE="2">
{
	<A HREF="index.cfm">Back to Product List</A>
}
</FONT>

<P>

<!--- If the price is not 0, display it --->
<CFIF len(trim(dblPrdCost))>
<!--- Use the function DollarFormat() around the product price to produce the proper formatting --->
<B>Price:</B> #dollarformat(dblPrdCost)#
<BR>
</CFIF>




<!--- Use the function ParagraphFormat() around the  product description to ensure proper formatting. this function replaces empty lines with the <P> tag in html --->

#ParagraphFormat(glbPrdDesc)#



</CFOUTPUT>
<!--- Include the Global Footer --->
<CFINCLUDE TEMPLATE="../CommonFiles/_footer.cfm">







