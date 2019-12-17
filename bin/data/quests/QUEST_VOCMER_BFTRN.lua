QUEST_VOCMER_BFTRN = {
	title = 'IDS_PROPQUEST_INC_000670',
	character = 'MaFl_Mustang',
	start_requirements = {
		min_level = 15,
		max_level = 15,
		job = { 'JOB_VAGRANT' },
	},
	rewards = {
		gold = 1500,
	},
	dialogs = {
		begin = {
			'IDS_PROPQUEST_INC_000671',
			'IDS_PROPQUEST_INC_000672',
			'IDS_PROPQUEST_INC_000673',
			'IDS_PROPQUEST_INC_000674',
		},
		begin_yes = {
			'IDS_PROPQUEST_INC_000675',
		},
		begin_no = {
			'IDS_PROPQUEST_INC_000676',
		},
		completed = {
			'IDS_PROPQUEST_INC_000677',
			'IDS_PROPQUEST_INC_000678',
		},
		not_finished = {
			'IDS_PROPQUEST_INC_000679',
			'IDS_PROPQUEST_INC_000680',
			'IDS_PROPQUEST_INC_000681',
		},
	}
}