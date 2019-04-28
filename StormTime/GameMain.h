#pragma once
#include <SFML/Graphics.hpp>

#include "MainScreen.h"

class GameMain
{
private:
	Screens::MainScreen* _mainScreen;

public:
	GameMain();
	~GameMain();

	void create() const;

	void render(sf::RenderWindow* window) const;
	void update(const float deltaTime) const;
};

