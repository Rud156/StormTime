#include "GameMain.h"


GameMain::GameMain()
{
	_controlsManager = Managers::ControlsManager::Instance();

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

void GameMain::handleKeyPress(int keyCode) const
{
	_controlsManager->handleKeyPress(keyCode);
}

void GameMain::handleKeyRelease(int keyCode) const
{
	_controlsManager->handleKeyReleased(keyCode);
}
