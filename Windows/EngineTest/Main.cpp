
#pragma comment(lib, "Engine.lib")


#define TEST_ENTITY_COMPONENTS 1

#if TEST_ENTITY_COMPONENTS
#include "TestEntityComponents.h"
#else
#error One of the TEST_ defines must be set to 1
#endif

int main()
{
#if _DEBUG
	// Check for memory leaks
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF );
#endif

	EngineTest test;
	if (test.initialize())
	{
		test.run();
		test.shutdown();
	}

	return 0;
	
}