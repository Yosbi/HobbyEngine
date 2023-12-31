#pragma once

#include "Test.h"
#include "..\Engine\Components\Entity.h"
#include "..\Engine\Components\Transform.h"

#include <iostream>
#include <ctime>

using namespace hobby;

class EngineTest : public Test
{
public:
	bool initialize() override
	{
		srand((u32)time(nullptr));
		return true;
	}

	void run() override
	{
		do {
			for (u32 i = 0; i < 10000; i++)
			{
				_create_random();
				_remove_random();
				_num_entities = (u32)_entities.size();
			}
			_print_results();

		} while(getchar() != 'q');
	}

	void shutdown() override
	{
	}

private:

	void _create_random()
	{
		u32 count = rand() % 20;

		if (_entities.empty()) 
			count = 1000;

		transform::init_info transform_info;
		game_entity::entity_info entity_info;
		entity_info.transform = &transform_info;

		while (count > 0)
		{
			_added++;
			game_entity::entity entity = game_entity::create_game_entity(entity_info);
			assert(entity.is_valid() && id::is_valid(entity.get_id()));
			_entities.push_back(entity);
			assert(game_entity::is_alive(entity));
			count--;
		}
	}

	void _remove_random()
	{
		u32 count = rand() % 20;

		if (_entities.size() < 1000)
			return;

		while (count > 0)
		{
			const u32 index = rand() % _entities.size();
			const game_entity::entity entity = _entities[index];
			assert(entity.is_valid() && id::is_valid(entity.get_id()));
			if (entity.is_valid())
			{
				game_entity::remove_game_entity(entity);
				_entities.erase(_entities.begin() + index);
				assert(!game_entity::is_alive(entity));
				_removed++;
			}

			count--;
		}
	}

	void _print_results()
	{
		std::cout << "Entities created: " << _added << std::endl;
		std::cout << "Entities removed: " << _removed << std::endl;
	}

	utl::vector<game_entity::entity> _entities;

	u32 _added = 0;
	u32 _removed = 0;
	u32 _num_entities = 0;

};