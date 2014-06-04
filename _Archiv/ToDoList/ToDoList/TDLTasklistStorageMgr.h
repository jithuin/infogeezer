// TDLTasklistStorageMgr.h: interface for the CTDLTasklistStorageMgr class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_TDLTASKLISTSTORAGEMGR_H__3FD3D340_A2C3_415F_BBFF_A407E3A06715__INCLUDED_)
#define AFX_TDLTASKLISTSTORAGEMGR_H__3FD3D340_A2C3_415F_BBFF_A407E3A06715__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\SHARED\TaskListStorageMgr.h"

class CTDLTasklistStorageMgr : public CTasklistStorageMgr  
{
public:
	CTDLTasklistStorageMgr();
	virtual ~CTDLTasklistStorageMgr();

protected:
	virtual void Initialize() const;

};

#endif // !defined(AFX_TDLTASKLISTSTORAGEMGR_H__3FD3D340_A2C3_415F_BBFF_A407E3A06715__INCLUDED_)
