/*
	Author  :    Nishant Sivakumar
	Date    :    June 9, 2005
	Info    :    Template class that makes it easy to use data allocated
				 in a different process. Useful when making inter-process
				 SendMessage/PostMessage calls.
	Contact :    nish#voidnish.com
*/

//ProcessData.h

#pragma once
#include <windows.h>
#include <assert.h>

template<typename T> class CProcessData
{
public:
	/*
		If you pass in a dwProcessId of 0, the current process Id is used.
		For the other arguments, see MSDN documentation for OpenProcess and
		VirtualAllocEx.
	*/
	CProcessData(DWORD dwProcessId = 0,
		DWORD dwDesiredAccess = PROCESS_ALL_ACCESS,
		DWORD flAllocationType = MEM_COMMIT, DWORD flProtect = PAGE_READWRITE)
	{
		m_hProcess = OpenProcess(dwDesiredAccess, FALSE,
			dwProcessId ? dwProcessId : GetCurrentProcessId());
		assert(m_hProcess);
		if (m_hProcess)
		{
			m_lpData = VirtualAllocEx(m_hProcess, NULL, sizeof T,
				flAllocationType, flProtect);
			assert(m_lpData);
		}
	}

	~CProcessData()
	{
		if (m_hProcess)
		{
			if (m_lpData)
			{
				VirtualFreeEx(m_hProcess, m_lpData, NULL, MEM_RELEASE);
			}
			else 
			{
				CloseHandle(m_hProcess);
			}
		}
	}

	//WriteData is used to copy data to memory in the foreign process
	BOOL WriteData(const T& data)
	{
		return (m_hProcess && m_lpData) ? WriteProcessMemory(
			m_hProcess, m_lpData,
			(LPCVOID)& data, sizeof T, NULL) : FALSE;
	}

	//ReadData reads back data from memory in the foreign process
	BOOL ReadData(T* data)
	{
		return (m_hProcess && m_lpData) ? ReadProcessMemory(
			m_hProcess, m_lpData,
			(LPVOID)data, sizeof T, NULL) : FALSE;
	}

	//Templated ReadData that's used to read a specific data type from
	//a memory address located in the foreign process
	template<typename TSUBTYPE> BOOL ReadData(
		TSUBTYPE* data, LPCVOID lpData)
	{
		return m_hProcess ? ReadProcessMemory(m_hProcess, lpData,
			(LPVOID)data, sizeof TSUBTYPE, NULL) : FALSE;
	}

	//Gets the address of the allocated memory in the foreign process
	const T* GetData()
	{
		return (m_hProcess && m_lpData) ? (T*)m_lpData : NULL;
	}
private:
	T m_Data;
	HANDLE m_hProcess;
	LPVOID m_lpData;
};
