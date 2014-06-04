//Written for the ToDoList (http://www.codeproject.com/KB/applications/todolist2.aspx)
//Design and latest version - http://www.codeproject.com/KB/applications/TDL_Calendar.aspx
//by demon.code.monkey@googlemail.com

#ifndef _CALENDARDEFINES_H_
#define _CALENDARDEFINES_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000


#define CALENDAR_DLL_NAME	"CalendarExt.dll"

#define CALENDAR_MSG_SELECTDATE			23356
#define CALENDAR_MSG_MOUSEWHEEL_UP		23357
#define CALENDAR_MSG_MOUSEWHEEL_DOWN	23358
#define CALENDAR_MSG_SELECTTASK    		23359

enum // visual style of completed tasks
{
	COMPLETEDTASKS_HIDDEN			= 0x0001,
	COMPLETEDTASKS_STRIKETHRU		= 0x0002,
	COMPLETEDTASKS_GREY				= 0x0004,

	SHOW_INCOMPLETE_STARTDATES		= 0x0100,
	SHOW_INCOMPLETE_DUEDATES		= 0x0200,
	SHOW_COMPLETE_STARTDATES		= 0x0400,
	SHOW_COMPLETE_DUEDATES			= 0x0800,
	SHOW_COMPLETE_DONEDATES			= 0x1000,

	DISPLAY_DEFAULT					= (0xffff ^ COMPLETEDTASKS_HIDDEN),
};

#endif//_CALENDARDEFINES_H_
