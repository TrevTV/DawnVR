// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <winsock2.h>
#include <ws2tcpip.h>
#include <windows.h>
#include <tchar.h>
#include <strsafe.h>
#include <string>
#define HTTP_IMPLEMENTATION
#include "./http.h"

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

struct in_addr BIN_IPV4_ADDR_LOOPBACK = { 127, 0, 0, 1 };
struct in6_addr BIN_IPV6_ADDR_LOOPBACK = { 0x0, 0x0,
											 0x0, 0x0,
											 0x0, 0x0,
											 0x0, 0x0,
											 0x0, 0x0,
											 0x0, 0x0,
											 0x0, 0x0,
											 0x0, 0x1 };
#define   MAX_LOCAL_NAME_LEN               64

extern "C" __declspec(dllexport) bool ConnectedToInternet() {
	BOOL bFoundLocalAddr = FALSE;
	char szAddrASCII[MAX_LOCAL_NAME_LEN];
	ADDRINFO AddrHints, * pAI, * pAddrInfo;

	//
	// Get the local host's name in ASCII text.
	//
	if (gethostname(szAddrASCII, MAX_LOCAL_NAME_LEN - 1))
	{
		//Print(TEXT("Error getting local host name, error = %d"), WSAGetLastError());
		return FALSE;
	}

	//
	// To obtain a list of all the local
	// addresses, resolve the local name with getaddrinfo for all
	// protocol families.
	//

	memset(&AddrHints, 0, sizeof(AddrHints));
	AddrHints.ai_family = PF_UNSPEC;
	AddrHints.ai_flags = AI_PASSIVE;

	if (getaddrinfo(szAddrASCII, "10", &AddrHints, &pAddrInfo))
	{
		//Print(TEXT("getaddrinfo(%hs) error %d"), szAddrASCII, WSAGetLastError());
		return FALSE;
	}

	//
	// Search the addresses returned.
	// If any of them match the loopback address, then
	// are not connected to an outside network.
	//
	// Note: This will not tell you how many networks you
	// are connected to.  If one or more networks are present,
	// then the loopback addresses will not be included in the
	// list returned from getaddrinfo.
	//

	bFoundLocalAddr = TRUE;
	for (pAI = pAddrInfo; pAI != NULL && bFoundLocalAddr; pAI = pAI->ai_next)
	{
		if (pAI->ai_family == PF_INET)
		{
			if (memcmp(&(((SOCKADDR_IN*)(pAI->ai_addr))->sin_addr), &BIN_IPV4_ADDR_LOOPBACK, sizeof(BIN_IPV4_ADDR_LOOPBACK)) == 0)
				bFoundLocalAddr = FALSE;
		}
		else if (pAI->ai_family == PF_INET6)
		{
			if (memcmp(&(((SOCKADDR_IN6*)(pAI->ai_addr))->sin6_addr), &BIN_IPV6_ADDR_LOOPBACK, sizeof(BIN_IPV6_ADDR_LOOPBACK)) == 0)
				bFoundLocalAddr = FALSE;
		}
	}

	freeaddrinfo(pAddrInfo);

	return bFoundLocalAddr;
}

extern "C" __declspec(dllexport) char const* LatestDawnRelease(char const* gitUrl) {
	http_t* request = http_get(gitUrl, NULL);
	if (!request)
	{
		//printf("Invalid request.\n");
		return "0.0.0.1";
	}

	http_status_t status = HTTP_STATUS_PENDING;
	int prev_size = -1;
	while (status == HTTP_STATUS_PENDING)
	{
		status = http_process(request);
		if (prev_size != (int)request->response_size)
		{
			//printf("%d byte(s) received.\n", (int)request->response_size);
			prev_size = (int)request->response_size;
		}
	}

	if (status == HTTP_STATUS_FAILED)
	{
		//printf("HTTP request failed (%d): %s.\n", request->status_code, request->reason_phrase);
		http_release(request);
		return "0.0.0.2";
	}

	//printf("\nContent type: %s\n\n%s\n", request->content_type, (char const*)request->response_data);
	return (char const*)request->response_data;
	http_release(request);
}

