#include "SharedTexturesManager.h"
#include "AssetManager.h"

namespace  Managers
{
	SharedTexturesManager* SharedTexturesManager::_instance = nullptr;

	SharedTexturesManager::SharedTexturesManager() = default;

	SharedTexturesManager::~SharedTexturesManager() = default;

	SharedTexturesManager* SharedTexturesManager::Instance()
	{
		if (_instance == nullptr)
			_instance = new SharedTexturesManager();

		return _instance;
	}

	void SharedTexturesManager::loadAllTextures()
	{
		for (int i = Background; i != Square_4; i++)
		{
			sf::Texture texture;
			const auto textureType = static_cast<TextureType>(i);
			if (texture.loadFromFile(AssetManager::texturePath(textureType)))
			{
				_textures.insert(std::pair<TextureType, sf::Texture>(textureType, texture));
			}
			else
			{
				exit(0);
			}
		}
	}

	sf::Texture SharedTexturesManager::getTexture(TextureType textureType)
	{
		return _textures[textureType];
	}
}
