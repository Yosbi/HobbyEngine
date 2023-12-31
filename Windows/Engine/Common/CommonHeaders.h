#pragma once

#pragma warning(disable: 4530) // disable exception warning

// C/C++
#include <stdint.h>
#include <assert.h>
#include <typeinfo>

#if defined(_WIN64)
#include <DirectXMath.h>
#endif

// common headers
#include "..\Utils\Utils.h"
#include "..\Utils\MathTypes.h"
#include "PrimitiveTypes.h"
