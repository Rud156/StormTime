#pragma once
#include <SFML/Graphics.hpp>
#include "Screens/MainScreen.h"
#include "Managers/ControlsManager.h"

class GameMain
{
private:
	Managers::ControlsManager* _controlsManager;

	Screens::MainScreen* _mainScreen;

public:
	GameMain();
	~GameMain();

	void create() const;

	void render(sf::RenderWindow* window) const;
	void update(float deltaTime) const;

	void handleKeyPress(int keyCode) const;
	void handleKeyRelease(int keyCode) const;
};

