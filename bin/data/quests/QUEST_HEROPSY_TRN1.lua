QUEST_HEROPSY_TRN1 = {
	title = 'IDS_PROPQUEST_INC_001599',
	character = 'MaDa_Cylor',
	start_requirements = {
		min_level = 60,
		max_level = 60,
		job = { 'JOB_MAGICIAN' },
	},
	rewards = {
		gold = 0,
	},
	dialogs = {
		begin = {
			'IDS_PROPQUEST_INC_001600',
			'IDS_PROPQUEST_INC_001601',
			'IDS_PROPQUEST_INC_001602',
		},
		begin_yes = {
			'IDS_PROPQUEST_INC_001603',
		},
		begin_no = {
			'IDS_PROPQUEST_INC_001604',
		},
		completed = {
			'IDS_PROPQUEST_INC_001605',
		},
		not_finished = {
			'IDS_PROPQUEST_INC_001606',
		},
	}
}