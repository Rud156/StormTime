#pragma once

#include <unordered_map>
#include <SFML/Graphics.hpp>

namespace  Managers
{
	class SharedTexturesManager
	{
	public:
		enum TextureType
		{
			Background,

			Circle_1,
			Circle_2,
			Circle_3,
			Circle_4,

			Square_1,
			Square_2,
			Square_3,
			Square_4,
		};

		~SharedTexturesManager();
		static  SharedTexturesManager* Instance();

		void loadAllTextures();

		sf::Texture getTexture(TextureType textureType);

	private:
		static SharedTexturesManager* _instance;
		SharedTexturesManager();

		std::unordered_map<TextureType, sf::Texture> _textures;
	};
}
