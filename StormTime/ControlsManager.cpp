#include "ControlsManager.h"

namespace Managers {
	ControlsManager* ControlsManager::_instance = nullptr;

	ControlsManager::ControlsManager()
	{
		for (int i = sf::Keyboard::Key::A; i != sf::Keyboard::Key::Pause; i++)
		{
			_keyStates.push_back(ControlsManager::KeyState::Released);
		}
	}

	ControlsManager::~ControlsManager()
	{
	}

	ControlsManager* ControlsManager::Instance()
	{
		if (_instance == nullptr)
			_instance = new ControlsManager();

		return _instance;
	}

	ControlsManager::KeyState ControlsManager::getKeyState(int keyCode)
	{
		return _keyStates[keyCode];
	}

	void ControlsManager::handleKeyPress(int keyCode)
	{
		_keyStates[keyCode] = ControlsManager::KeyState::JustPressed;
	}

	void ControlsManager::handleKeyReleased(int keyCode)
	{
		_keyStates[keyCode] = ControlsManager::KeyState::JustReleased;
	}

	void ControlsManager::update()
	{
	}
}