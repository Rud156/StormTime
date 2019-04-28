#pragma once
#include <SFML/Graphics/RenderWindow.hpp>

namespace Screens
{
	class CustomScreen
	{
	public:
		virtual ~CustomScreen() = default;

		virtual void create() = 0;

		virtual void draw(sf::RenderWindow* window) = 0;

		virtual void update(const float deltaTime) = 0;
	};
}
