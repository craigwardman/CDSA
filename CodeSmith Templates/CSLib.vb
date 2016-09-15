imports CodeSmith.Engine
imports System.IO
imports Microsoft.VisualBasic

partial public class CSLib
	inherits CodeTemplate

	public shared function GetFormattedName(byval name as string) as string
		dim formattedName as string=name
		
		'remove known prefixes
		dim knownPrefixes as String() = { "tbl_", "int_", "dbl_", "dt_", "str_" }
		
		for each knownPrefix as String in knownPrefixes
			if formattedName.StartsWith(knownPrefix,true, nothing) then formattedName=formattedName.Substring(knownPrefix.length, formattedName.Length-knownPrefix.length)	
		next
		
		'remove non alphanumeric characters
		Dim rgx As New System.Text.RegularExpressions.Regex("[^a-zA-Z0-9_]")
        formattedName = rgx.Replace(formattedName, "")
		
		'make sure the first character isnt numeric
		if IsNumeric (formattedName.Substring(0,1)) then formattedName = "N" & formattedName
		
		'make sure the first character isnt an underscore (reserved for private)
		if formattedName.Substring(0,1)="_" then formattedName=formattedName.Substring(1,formattedName.Length-1)
		
		'capitalise the first char
		formattedName=formattedName.Substring(0,1).ToUpper() & formattedName.Substring(1,formattedName.Length-1)
		
		return formattedName
	end function
	
	'use CType(obj, typeName) instead
	'public shared function GetTypeConverter(byval returnType as System.Type) as string
	'	dim name as string=GetTypeName(returnType)
	'	
	'	return "Convert.To" & name.substring(name.LastIndexOf(".")+1,name.Length-name.LastIndexOf(".")-1)
	'end function
	
	public shared function GetTypeName(byval systemType as System.Type, byval nullable as boolean) as string
		dim name as string
		
		'not all types are nice to .NET, filter them here
		if systemType is GetType(System.Xml.XmlDocument) then
			name=GetType(String).toString()
		else
			'make sure no square brackets (for arrays)
			'name=systemType.ToString().Replace("[","(").Replace("]",")")
			name=systemType.ToString()
		end if
			
		if nullable and systemType.IsValueType then name &= "?"
		return name
	end function

	public shared sub SafeCreateDirectory(byval path as string)
			if not Directory.Exists(path) then
				Directory.CreateDirectory(path)
			end if
	end sub
	
end class
