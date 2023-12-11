#pragma once
#include "ComponentsCommon.h"

namespace hobby::transform {

	struct init_info {
		f32 position[3] = {};
		f32 rotation[4] = {};
		f32 scale[3] = {1.0f, 1.0f, 1.0f};
	};

	transform_id create_transform(const init_info& info, game_entity::entity_id entity_id);
	void remove_transform(transform_id id);
}