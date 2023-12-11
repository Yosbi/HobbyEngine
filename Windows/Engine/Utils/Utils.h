#pragma once

#define USE_STL_VECTOR 1
#define USE_STL_DEQUE 1

#if USE_STL_VECTOR
#include <vector>

namespace hobby::utl
{
	template<typename T>
	using vector = std::vector<T>;
}
#endif

#if USE_STL_DEQUE
#include <deque>

namespace hobby::utl
{
	template<typename T>
	using deque = std::deque<T>;
}
#endif

namespace hobby::utl {
	// TODO: Implement a custom vector and deque
}