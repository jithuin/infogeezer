// TDCSearchParamHelper.cpp: implementation of the CTDCSearchParamHelper class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "TDCSearchParamHelper.h"
#include "TDCCustomattributehelper.h"

#include "..\shared\Preferences.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

BOOL CTDCSearchParamHelper::LoadRule(const CPreferences& prefs, const CString& sRule,
									const CTDCCustomAttribDefinitionArray& aCustomAttribDefs, 
									SEARCHPARAM& rule)
{
	CString sAttribID = prefs.GetProfileString(sRule, _T("Attribute"));
	FIND_OPERATOR op = (FIND_OPERATOR)prefs.GetProfileInt(sRule, _T("Operator"), FO_EQUALS);
	BOOL bAnd = prefs.GetProfileInt(sRule, _T("And"), TRUE);
	DWORD dwFlags = prefs.GetProfileInt(sRule, _T("Flags"), 0);
	CString sValue = prefs.GetProfileString(sRule, _T("Value"));

	TDC_ATTRIBUTE attrib = TDCA_NONE;
	FIND_ATTRIBTYPE nAttribType = FT_NONE;
	CString sCustAttribID;
	
	if (!DecodeAttribute(sAttribID, dwFlags, aCustomAttribDefs, attrib, sCustAttribID, nAttribType))
	{
		return FALSE;
	}
	else if (!sCustAttribID.IsEmpty())
	{
		VERIFY(rule.Set(attrib, sCustAttribID, nAttribType, op, sValue, bAnd));
		return TRUE;
	}

	// else
	VERIFY(rule.Set(attrib, op, sValue, bAnd, nAttribType));
	return TRUE;
}

BOOL CTDCSearchParamHelper::SaveRule(CPreferences& prefs, const CString& sRule, const SEARCHPARAM& rule)
{
	// check for invalid attribute
	if (rule.GetAttribute() == TDCA_NONE)
		return FALSE;

	// handle custom attributes
	if (rule.IsCustomAttribute())
	{
		CString sAttrib;
		sAttrib.Format(_T("%d.%s"), rule.GetAttribute(), rule.GetCustomAttributeID());

		prefs.WriteProfileString(sRule, _T("Attribute"), sAttrib);
	}
	else
	{
		prefs.WriteProfileInt(sRule, _T("Attribute"), rule.GetAttribute());
	}

	prefs.WriteProfileInt(sRule, _T("Operator"), rule.GetOperator());
	prefs.WriteProfileInt(sRule, _T("And"), rule.GetAnd());
	prefs.WriteProfileString(sRule, _T("Value"), rule.ValueAsString());
	prefs.WriteProfileInt(sRule, _T("Flags"), rule.GetFlags());

	return TRUE;
}

BOOL CTDCSearchParamHelper::DecodeAttribute(const CString& sAttrib, DWORD dwFlags,
											const CTDCCustomAttribDefinitionArray& aCustAttribDefs, 
											TDC_ATTRIBUTE& nAttrib, CString& sUniqueID,
											FIND_ATTRIBTYPE& nFindType)
{
	nAttrib = TDCA_NONE;
	nFindType = FT_NONE;
	sUniqueID.Empty();

	CStringArray aParts;
	int nNumParts = Misc::Split(sAttrib, '.', aParts);

	switch (nNumParts)
	{
	case 1:
		if (Misc::IsNumber(sAttrib))
		{
			nAttrib = (TDC_ATTRIBUTE)_ttoi(sAttrib);
			ASSERT (nAttrib != FT_NONE);

			if (SEARCHPARAM::IsCustomAttribute(nAttrib))
			{
				sUniqueID = CTDCCustomAttributeHelper::GetAttributeTypeID(nAttrib, aCustAttribDefs);
				ASSERT(!sUniqueID.IsEmpty());
	
				nFindType = CTDCCustomAttributeHelper::GetAttributeFindType(sUniqueID, dwFlags, aCustAttribDefs);
			}
			else
			{
				nFindType = SEARCHPARAM::GetAttribType(nAttrib, dwFlags);
			}

			ASSERT(nFindType != FT_NONE);
		}
		else
		{
			sUniqueID = sAttrib;
			ASSERT(!sUniqueID.IsEmpty());

			nAttrib = CTDCCustomAttributeHelper::GetAttributeID(sUniqueID, aCustAttribDefs);

			// fallback
			if (nAttrib == FT_NONE)
			{
				nAttrib = TDCA_CUSTOMATTRIB;
			}

			ASSERT(SEARCHPARAM::IsCustomAttribute(nAttrib));
			
			nFindType = CTDCCustomAttributeHelper::GetAttributeFindType(sUniqueID, dwFlags, aCustAttribDefs);
			ASSERT(nFindType != FT_NONE);
		}
		break;
		
	case 2:
		nAttrib = (TDC_ATTRIBUTE)_ttoi(aParts[0]);
		ASSERT(SEARCHPARAM::IsCustomAttribute(nAttrib));
		
		sUniqueID = aParts[1];
		ASSERT(!sUniqueID.IsEmpty());

		nFindType = CTDCCustomAttributeHelper::GetAttributeFindType(sUniqueID, dwFlags, aCustAttribDefs);
		ASSERT(nFindType != FT_NONE);
		break;

	case 0:
	default:
		break;
	}

	return (nAttrib != TDCA_NONE);
}
