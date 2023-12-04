#include "Entity.h"

namespace hobby::game_entity
{
	namespace {
		std::vector<id::generation_type> generations;

	}// Anonymous namespace

	entity_id create_game_entity(const entity_info& info)
	{
		assert(info.transform); // All game entities must have a transform component
		if (!info.transform) return entity_id{ id::invalid_id };
	}

	void remove_game_entity(entity_id id)
	{
		generations[id::index(id)] = id::new_generation(generations[id::index(id)]);
	}

	bool is_alive(entity_id id)
	{
		return generations[id::index(id)] == id::generation(id);
	}
}