#include "GameMain.h"


GameMain::GameMain()
{
	_mainScreen = Screens::MainScreen::Instance();
}


GameMain::~GameMain()
{
}

void GameMain::create() const
{
	_mainScreen->create();
}

void GameMain::render(sf::RenderWindow* window) const
{
	_mainScreen->draw(window);
}

void GameMain::update(const float deltaTime) const
{
	_mainScreen->update(deltaTime);
}
