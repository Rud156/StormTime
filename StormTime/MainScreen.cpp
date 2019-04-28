#include "MainScreen.h"

namespace  Screens
{
	MainScreen* MainScreen::_instance = nullptr;

	MainScreen::MainScreen()
	{
	}

	MainScreen::~MainScreen()
	{
	}

	MainScreen* MainScreen::Instance()
	{
		if (_instance == nullptr)
			_instance = new MainScreen();

		return _instance;
	}

	void MainScreen::create()
	{
	}

	void MainScreen::draw(sf::RenderWindow* window)
	{
	}

	void MainScreen::update(const float deltaTime)
	{
	}
}
