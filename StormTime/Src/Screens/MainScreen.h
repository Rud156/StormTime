#pragma once
#include "CustomScreen.h"

namespace Screens {
	class MainScreen final : public CustomScreen
	{
	private:
		static MainScreen* _instance;
		MainScreen();

	public:
		~MainScreen();
		static MainScreen* Instance();
		
		void create() override;
		
		void draw(sf::RenderWindow* window) override;
		
		void update(const float deltaTime) override;
	};
}